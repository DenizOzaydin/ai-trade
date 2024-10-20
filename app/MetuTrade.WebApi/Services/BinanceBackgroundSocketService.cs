using Binance.Net.Clients;
using Binance.Net.Interfaces;
using Binance.Net.Interfaces.Clients;
using CryptoExchange.Net.Objects.Sockets;
using MetuTrade.Business.Mapper;
using MetuTrade.Core.Entities;
using MetuTrade.DataAccess;
using MetuTrade.WebApi.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace MetuTrade.WebApi.Services
{
    public class BinanceBackgroundSocketService
    {
        private readonly ConcurrentDictionary<Guid, BinanceKlineSubscription> _subscriptions;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHubContext<AdminHub> _adminHub;
        private readonly IBinanceSocketClient _binanceSocketClient;

        public BinanceBackgroundSocketService(IServiceProvider serviceProvider, IHubContext<AdminHub> adminHub, IBinanceSocketClient binanceSocketClient)
        {
            _serviceProvider = serviceProvider;
            _adminHub = adminHub;
            _subscriptions = new();
            _binanceSocketClient = binanceSocketClient;
        }

        public async Task SubscribeKlinesAsync(string symbol, string interval)
        {
            Action<DataEvent<IBinanceStreamKlineData>> onMessage = new Action<DataEvent<IBinanceStreamKlineData>>(async data =>
            {
                Bar bar = MapperProfile.MapBar(symbol, interval, data.Data.Data);

                using (var scope = _serviceProvider.CreateScope())
                {
                    BarRepository barRepository = scope.ServiceProvider.GetRequiredService<BarRepository>();
                    await barRepository.UpdateAsync(bar);
                    await barRepository.SaveChangesAsync();
                }
            });

            var result = await _binanceSocketClient.SpotApi.ExchangeData.SubscribeToKlineUpdatesAsync(symbol, MapperProfile.MapInterval(interval), onMessage: onMessage);
            var subscription = result.Data;

            BinanceKlineSubscription klineSubscription = new BinanceKlineSubscription
            {
                Id = Guid.NewGuid(),
                Subscription = subscription,
                Symbol = symbol,
                Interval = interval,
                Status = BinanceSubscriptionStatus.Connected
            };

            Guid id = klineSubscription.Id;
            _subscriptions.GetOrAdd(id, klineSubscription);

            subscription.Exception += async ex =>
            {
                if (_subscriptions.ContainsKey(id))
                {
                    _subscriptions[id].ErrorMessage = ex.Message;
                    await _adminHub.Clients.All.SendAsync("SendSubscriptionStatusMessage", new BinanceSubscriptionMessage
                    {
                        Id = _subscriptions[id].Id,
                        Symbol = _subscriptions[id].Symbol,
                        Interval = _subscriptions[id].Interval,
                        Status = _subscriptions[id].Status.ToString(),
                        ErrorMessage = _subscriptions[id].ErrorMessage
                    });
                }
            };

            subscription.ConnectionLost += async () =>
            {
                if (_subscriptions.ContainsKey(id))
                {
                    _subscriptions[id].Status = BinanceSubscriptionStatus.ConnectionLost;
                    await _adminHub.Clients.All.SendAsync("SendSubscriptionStatusMessage", new BinanceSubscriptionMessage
                    {
                        Id = _subscriptions[id].Id,
                        Symbol = _subscriptions[id].Symbol,
                        Interval = _subscriptions[id].Interval,
                        Status = _subscriptions[id].Status.ToString(),
                        ErrorMessage = _subscriptions[id].ErrorMessage
                    });
                }
            };

            subscription.ConnectionRestored += async ts =>
            {
                if (_subscriptions.ContainsKey(id))
                {
                    _subscriptions[id].Status = BinanceSubscriptionStatus.Connected;
                    await _adminHub.Clients.All.SendAsync("SendSubscriptionStatusMessage", new BinanceSubscriptionMessage
                    {
                        Id = _subscriptions[id].Id,
                        Symbol = _subscriptions[id].Symbol,
                        Interval = _subscriptions[id].Interval,
                        Status = _subscriptions[id].Status.ToString(),
                        ErrorMessage = _subscriptions[id].ErrorMessage
                    });
                }
            };

            subscription.ConnectionClosed += async () =>
            {
                if (_subscriptions.ContainsKey(id))
                {
                    _subscriptions[id].Status = BinanceSubscriptionStatus.ConnectionClosed;
                    await _adminHub.Clients.All.SendAsync("SendSubscriptionStatusMessage", new BinanceSubscriptionMessage
                    {
                        Id = _subscriptions[id].Id,
                        Symbol = _subscriptions[id].Symbol,
                        Interval = _subscriptions[id].Interval,
                        Status = _subscriptions[id].Status.ToString(),
                        ErrorMessage = _subscriptions[id].ErrorMessage
                    });
                }
            };

            subscription.ResubscribingFailed += async err =>
            {
                if (_subscriptions.ContainsKey(id))
                {
                    _subscriptions[id].Status = BinanceSubscriptionStatus.ConnectionLost;
                    _subscriptions[id].ErrorMessage = err.Message;
                    await _adminHub.Clients.All.SendAsync("SendSubscriptionStatusMessage", new BinanceSubscriptionMessage
                    {
                        Id = _subscriptions[id].Id,
                        Symbol = _subscriptions[id].Symbol,
                        Interval = _subscriptions[id].Interval,
                        Status = _subscriptions[id].Status.ToString(),
                        ErrorMessage = _subscriptions[id].ErrorMessage
                    });
                }
            };
        }

        public List<BinanceSubscriptionMessage> GetAllSubscriptions()
        {
            var subscriptions = _subscriptions.Values.ToList();

            List<BinanceSubscriptionMessage> messages = new();

            foreach(var subscription in subscriptions)
            {
                BinanceSubscriptionMessage message = new BinanceSubscriptionMessage
                {
                    Id = subscription.Id,
                    Symbol = subscription.Symbol,
                    Interval = subscription.Interval,
                    Status = subscription.Status.ToString(),
                    ErrorMessage = subscription.ErrorMessage
                };
                messages.Add(message);
            }

            return messages;
        }

        public async Task<bool> UnsubscribeKlinesAsync(Guid id)
        {
            BinanceKlineSubscription? subscription = null;

            _subscriptions.TryGetValue(id, out subscription);

            if(subscription != null)
            {
                await _binanceSocketClient.UnsubscribeAsync(subscription.Subscription.Id);
                return true;
            }

            return false;
        }
    } 
}

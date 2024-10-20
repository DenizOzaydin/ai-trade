using CryptoExchange.Net.Objects.Sockets;
using MetuTrade.WebApi.Services.Enums;

namespace MetuTrade.WebApi.Services.Tasks
{
    public class BinanceKlineSubscription
    {
        public Guid Id { get; set; }
        public string Symbol { get; set; }
        public string Interval { get; set; }
        public BinanceSubscriptionStatus Status { get; set; }
        public UpdateSubscription Subscription { get; set; }
        public string ErrorMessage { get; set; } = "";
    }
}

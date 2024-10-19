using MetuTrade.Business.Events;
using MetuTrade.Business.Results;
using MetuTrade.Business.Settings;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetuTrade.Business.WebSocket
{
    public class AdminClient
    {
        private HubConnection _hubConnection;
        public bool IsConnected { get; private set; } = false;

        private readonly AdminSettings _adminSettings;

        public AdminClient(IOptions<AdminSettings> adminSettings)
        {
            _adminSettings = adminSettings.Value;
        }

        public event EventHandler<DownloadInfoReceivedEventArgs> DownloadInfoReceived;
        public event EventHandler<Exception?> ConnectionClosed;
        public event EventHandler<Exception?> Reconnecting;
        public event EventHandler<string?> Reconnected;

        protected virtual void OnDownloadInfoReceived(DownloadInfoReceivedEventArgs e)
        {
            DownloadInfoReceived?.Invoke(this, e);
        }

        protected virtual void OnConnectionClosed(Exception? ex)
        {
            ConnectionClosed?.Invoke(this, ex);
        }

        protected virtual void OnReconnecting(Exception? ex)
        {
            Reconnecting?.Invoke(this, ex);
        }

        protected virtual void OnReconnected(string? arg)
        {
            Reconnected?.Invoke(this, arg);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (IsConnected) return;

            string url = Path.Combine(_adminSettings.BaseAddressUrl, _adminSettings.AdminHubUrl);

            _hubConnection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7269/hubs/admin")
                .Build();

            _hubConnection.On<DownloadOperationResult>("ReceiveMessage", result =>
            {
                DownloadInfoReceivedEventArgs e = new DownloadInfoReceivedEventArgs(result);
                OnDownloadInfoReceived(e);
            });

            _hubConnection.Closed += async ex =>
            {
                IsConnected = false;
                OnConnectionClosed(ex);
                await Task.CompletedTask;
            };

            _hubConnection.Reconnected += async str =>
            {
                IsConnected = true;
                OnReconnected(str);
                await Task.CompletedTask;
            };

            _hubConnection.Reconnecting += async ex =>
            {
                IsConnected = false;
                OnReconnecting(ex);
                await Task.CompletedTask;
            };

            try
            {
                await _hubConnection.StartAsync();
            }
            catch
            {

            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _hubConnection.StopAsync();
        }
    }
}

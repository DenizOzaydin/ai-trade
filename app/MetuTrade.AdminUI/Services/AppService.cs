using MetuTrade.Business.WebSocket;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MetuTrade.AdminUI.Services
{
    public class AppService : IDisposable, IHostedService
    {
        private readonly AdminClient _adminClient;
       
        public AppService(AdminClient adminClient)
        {
            _adminClient = adminClient;
        }

        public void Dispose()
        {
            
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _adminClient.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _adminClient.StopAsync(cancellationToken);
        }
    }
}

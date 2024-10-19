using MetuTrade.Business.Results;
using MetuTrade.WebApi.Services;
using Microsoft.AspNetCore.SignalR;

namespace MetuTrade.WebApi.Hubs
{
    public class AdminHub : Hub
    {
        public async Task SendDownloadOperationMessage(DownloadOperationResult result)
        {
            await Clients.All.SendAsync("ReceiveDownloadOperationMessage", result);
        }

        public async Task SendSubscriptionStatusMessage(BinanceSubscriptionMessage message)
        {
            await Clients.All.SendAsync("ReceiveSubscriptionMessage", message);
        }
    }
}

using MetuTrade.Business.Results;
using Microsoft.AspNetCore.SignalR;

namespace MetuTrade.WebApi.Hubs
{
    public class AdminHub : Hub
    {
        public async Task SendMessage(DownloadOperationResult result)
        {
            await Clients.All.SendAsync("ReceiveMessage", result);
        }
    }
}

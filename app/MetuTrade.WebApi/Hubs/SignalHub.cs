using MetuTrade.Business.Results;
using Microsoft.AspNetCore.SignalR;

namespace MetuTrade.WebApi.Hubs
{
    public class SignalHub : Hub
    {
        public async Task SendSignal(SignalResult result)
        {
            await Clients.All.SendAsync("ReceiveSignal", result);
        }
    }
}

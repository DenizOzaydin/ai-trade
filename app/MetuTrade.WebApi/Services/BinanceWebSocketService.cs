using MetuTrade.Business.Settings;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace MetuTrade.WebApi.Services
{
    public class BinanceWebSocketService
    {
        private readonly BinanceWssSettings _settings;

        public BinanceWebSocketService(IOptions<BinanceWssSettings> settings)
        {
            _settings = settings.Value;
        }
    }
}

using Binance.Net.Clients;
using MetuTrade.Business.Results;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace MetuTrade.AdminConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var restClient = new BinanceRestClient();
            var klines = restClient.SpotApi.ExchangeData.GetKlinesAsync("BTCUSDT", Binance.Net.Enums.KlineInterval.FiveMinutes, limit:1000).Result;
            Console.WriteLine(klines.Data.Count());
        }
    }
}

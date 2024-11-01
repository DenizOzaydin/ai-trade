using Binance.Net.Clients;
using MetuTrade.Business.Results;
using MetuTrade.Core.ArtificialIntelligence;
using MetuTrade.Core.TechnicalAnalysis;
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
            Console.ReadLine();

            ConnectAsync();

            Console.ReadLine();
        }

        public async static Task ConnectAsync()
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7269/hubs/signal")
                .Build();

            connection.On<SignalResult>("ReceiveSignal", (sr) =>
            {
                DateTime date = sr.LastUpdated;
                string symbol = sr.Symbol;
                double signal = sr.Signal ?? 0;
                Console.WriteLine($"{date} {symbol} {signal.ToString("0.00")}");
            });

            await connection.StartAsync();
        }
    }
}

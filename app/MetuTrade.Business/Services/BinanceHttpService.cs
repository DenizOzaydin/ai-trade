namespace MetuTrade.Business.Services;

using Binance.Net.Clients;
using Binance.Net.Enums;
using CryptoExchange.Net.SharedApis;
using MetuTrade.Business.Results;
using MetuTrade.Business.Settings;
using MetuTrade.Core;
using MetuTrade.DataAccess.Market;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

public class BinanceHttpService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly BarRepository _barRepository;
    private readonly BinanceSettings _settings;

    public BinanceHttpService(IHttpClientFactory httpClientFactory, BarRepository barRepository, IOptions<BinanceSettings> settings)
    {
        _httpClientFactory = httpClientFactory;
        _barRepository = barRepository;
        _settings = settings.Value;
    }

    public async Task<BinanceDownloadResult> DownloadAsync(string symbol, string interval, long startTime, long endTime)
    {
        BinanceDownloadResult result = new BinanceDownloadResult { Symbol = symbol, Count = 0, Interval = interval, StartTime = startTime, EndTime = endTime };
        KlineInterval klineInterval = MapInterval(interval);

        var restClient = new BinanceRestClient();

        var response = await restClient.SpotApi.ExchangeData.GetKlinesAsync(symbol, klineInterval, startTime: DateTimeOffset.FromUnixTimeMilliseconds(startTime), endTime: DateTimeOffset.FromUnixTimeMilliseconds(endTime)));

            var bars = new List<Bar>();

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                result.StatusCode = response.StatusCode;
                return result;
            }

            string contents = await response.Content.ReadAsStringAsync();

            List<List<object>>? model = JsonConvert.DeserializeObject<List<List<object>>>(contents);

            if (model == null)
            {
                result.StatusCode = response.StatusCode;
                return result;
            }

            long lastTime = 0;
            bool flag = false;
            long firstBarOpenTime = 0;

            foreach (var barModel in model)
            {
                Bar bar = new Bar();
                bar.Symbol = symbol;
                bar.Interval = interval;
                bar.Open = double.Parse((string)barModel[1]);
                bar.High = double.Parse((string)barModel[2]);
                bar.Low = double.Parse((string)barModel[3]);
                bar.Close = double.Parse((string)barModel[4]);
                bar.Volume = double.Parse((string)barModel[5]);
                bar.OpenTime = (long)barModel[0];

                if (!flag)
                {
                    firstBarOpenTime = bar.OpenTime;
                    flag = true;
                }

                lastTime = (long)barModel[6];

                await _barRepository.UpdateAsync(bar);

            lastTime++;
            await _barRepository.SaveChangesAsync();

            result.NextStartTime = lastTime;
            result.FirstBarOpenTime = firstBarOpenTime;
            result.Count = model.Count;

            result.StatusCode = System.Net.HttpStatusCode.OK;

            return result;
        }
    }

    public KlineInterval MapInterval(string interval)
    {
        switch(interval)
        {
            case "1-m":
                return KlineInterval.OneMinute;
            case "3-m":
                return KlineInterval.ThreeMinutes;
            case "5-m":
                return KlineInterval.FiveMinutes;
            case "15-m":
                return KlineInterval.FifteenMinutes;
            case "1-h":
                return KlineInterval.OneHour;
            case "4-h":
                return KlineInterval.FourHour;
            case "1-d":
                return KlineInterval.OneDay;
        }
        throw new ArgumentException($"Interval {interval} is not valid.");
    }

    public string ReverseMapInterval(KlineInterval interval)
    {
        switch(interval)
        {
            case KlineInterval.OneMinute:
                return "1-m";
            case KlineInterval.ThreeMinutes:
                return "3-m";
            case KlineInterval.FiveMinutes:
                return "5-m";
            case KlineInterval.FifteenMinutes:
                return "15-m";
            case KlineInterval.OneHour:
                return "1-h";
            case KlineInterval.FourHour:
                return "4-h";
            case KlineInterval.OneDay:
                return "1-d";
        }
        throw new ArgumentException($"Interval {interval} is not valid.");
    }
}
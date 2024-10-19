namespace MetuTrade.Business.Services;

using Binance.Net.Clients;
using Binance.Net.Enums;
using CryptoExchange.Net.SharedApis;
using MetuTrade.Business.Mapper;
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
        KlineInterval klineInterval = MapperProfile.MapInterval(interval);

        var restClient = new BinanceRestClient();

        var _response = await restClient.SpotApi.ExchangeData.GetKlinesAsync(symbol, klineInterval, startTime: DateTimeOffset.FromUnixTimeMilliseconds(startTime).UtcDateTime, endTime: DateTimeOffset.FromUnixTimeMilliseconds(endTime).UtcDateTime, limit:1000);

        var bars = new List<Bar>();

        if (!_response.Success)
        {
            result.StatusCode = _response.ResponseStatusCode ?? System.Net.HttpStatusCode.BadRequest;
            return result;
        }

        long lastTime = 0;
        bool flag = false;
        long firstBarOpenTime = 0;

        foreach (var barModel in _response.Data)
        {
            Bar bar = new Bar();
            bar.Symbol = symbol;
            bar.Interval = interval;
            bar.Open = (double)barModel.OpenPrice;
            bar.High = (double)barModel.HighPrice;
            bar.Low = (double)barModel.LowPrice;
            bar.Close = (double)barModel.ClosePrice;
            bar.Volume = (double)barModel.Volume;
            bar.OpenTime = ((DateTimeOffset)barModel.OpenTime).ToUnixTimeMilliseconds();

            if (!flag)
            {
                firstBarOpenTime = bar.OpenTime;
                flag = true;
            }

            lastTime = ((DateTimeOffset)barModel.CloseTime).ToUnixTimeMilliseconds();

            await _barRepository.UpdateAsync(bar);
        }

        lastTime++;
        await _barRepository.SaveChangesAsync();

        result.NextStartTime = lastTime;
        result.FirstBarOpenTime = firstBarOpenTime;
        result.Count = _response.Data.Count();

        result.StatusCode = System.Net.HttpStatusCode.OK;

        return result;
    }
}
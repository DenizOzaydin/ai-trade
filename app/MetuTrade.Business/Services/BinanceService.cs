namespace MetuTrade.Business.Services;

using MetuTrade.Business.Results;
using MetuTrade.Business.Settings;
using MetuTrade.Core;
using MetuTrade.DataAccess.Market;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

public class BinanceService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly BarRepository _barRepository;
    private readonly BinanceSettings _settings;

    public BinanceService(IHttpClientFactory httpClientFactory, BarRepository barRepository, IOptions<BinanceSettings> settings)
    {
        _httpClientFactory = httpClientFactory;
        _barRepository = barRepository;
        _settings = settings.Value;
    }

    public async Task<BinanceDownloadResult> DownloadAsync(string symbol, string interval, long startTime, long endTime)
    {
        BinanceDownloadResult result = new BinanceDownloadResult { Symbol = symbol, Count = 0, Interval = interval, StartTime = startTime, EndTime = endTime };

        string address = _settings.KlinesUrl;
        string query = Tools.CreateQuery(("symbol", symbol), ("interval", MapInterval(interval)), ("startTime", startTime), ("endTime", endTime), ("limit", 1000));
        string url = address + "?" + query;

        using (HttpClient client = _httpClientFactory.CreateClient())
        {
            client.BaseAddress = new Uri(_settings.BaseAddressUrl);

            var response = await client.GetAsync(url);
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
            }
            lastTime++;
            await _barRepository.SaveChangesAsync();

            result.NextStartTime = lastTime;
            result.FirstBarOpenTime = firstBarOpenTime;
            result.Count = model.Count;

            result.StatusCode = System.Net.HttpStatusCode.OK;

            return result;
        }
    }

    public string MapInterval(string interval)
    {
        string[] split = interval.Split('-');
        int mult = int.Parse(split[0]);
        string ts = split[1];

        if (ts == "s") ts = "s";
        else if (ts == "m") ts = "m";
        else if (ts == "h") ts = "h";
        else if (ts == "d") ts = "d";
        else if (ts == "M") ts = "M";
        else if (ts == "y") ts = "y";
        else throw new ArgumentException("Interval is invalid.");

        return $"{mult}{ts}";
    }

    public string ReverseMapInterval(string interval)
    {
        string mult = "";
        string ts = "";
        for (int i = 0; i < interval.Length; i++)
        {
            if (i != interval.Length - 1)
            {
                mult += interval[i];
            }
            else
            {
                ts += interval[i];
            }
        }

        return $"{mult}-{ts}";
    }
}
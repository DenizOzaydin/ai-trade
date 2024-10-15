using MetuTrade.Business.Settings;
using Microsoft.Extensions.Options;
using System.Net.Http;
using MetuTrade.Business.Results;
using MetuTrade.Business.RequestModels;
using MetuTrade.Core;
using Newtonsoft.Json;

namespace MetuTrade.Business.Services;

public class AdminService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AppSettings _settings;

    public AdminService(IHttpClientFactory httpClientFactory, IOptions<AppSettings> settings)
    {
        _httpClientFactory = httpClientFactory;
        _settings = settings.Value;
    }

    public async Task<DownloadStartResult> DownloadStartAsync(string symbol, string interval, string startDate, string endDate)
    {
        DownloadStartRequestModel model = new DownloadStartRequestModel
        {
            Symbol = symbol,
            Interval = interval,
            StartDate = startDate,
            EndDate = endDate
        };

        DownloadStartResult result = new DownloadStartResult();

        using (var client = _httpClientFactory.CreateClient())
        {
            client.BaseAddress = new Uri(_settings.BaseAddressUrl);

            var byteContent = Tools.GenerateByteContent(model);
            var response = await client.PostAsync(_settings.DownloadStartUrl, byteContent);

            result.StatusCode = response.StatusCode;
        }

        return result;
    }

    public async Task<List<DownloadOperationResult>> GetDownloadOperationsAsync()
    {
        List<DownloadOperationResult> result = new List<DownloadOperationResult>();

        using (var client = _httpClientFactory.CreateClient())
        {
            client.BaseAddress = new Uri(_settings.BaseAddressUrl);

            var response = await client.GetAsync(_settings.DownloadOperationsUrl);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<List<DownloadOperationResult>>(json);
            }
        }

        return result;
    }

    public async Task<DownloadCancelResult> DownloadCancelAsync(Guid taskId)
    {
        DownloadCancelRequestModel model = new DownloadCancelRequestModel
        {
            TaskId = taskId
        };

        DownloadCancelResult result = new DownloadCancelResult();

        using (var client = _httpClientFactory.CreateClient())
        {
            client.BaseAddress = new Uri(_settings.BaseAddressUrl);

            var byteContent = Tools.GenerateByteContent(model);
            var response = await client.PostAsync(_settings.DownloadCancelUrl, byteContent);

            result.StatusCode = response.StatusCode;
        }

        return result;
    }

    public async Task DeleteCanceledOperationsAsync()
    {
        using (var client = _httpClientFactory.CreateClient())
        {
            client.BaseAddress = new Uri(_settings.BaseAddressUrl);

            var response = await client.PostAsync(_settings.DownloadDeleteCanceledOperationsUrl, null);
        }
    }

    public async Task DeleteSucceededOperationsAsync()
    {
        using (var client = _httpClientFactory.CreateClient())
        {
            client.BaseAddress = new Uri(_settings.BaseAddressUrl);

            var response = await client.PostAsync(_settings.DownloadDeleteSucceededOperationsUrl, null);
        }
    }
}
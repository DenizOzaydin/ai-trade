namespace MetuTrade.WebApi.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MetuTrade.Business.RequestModels;
using MetuTrade.Business.Services;
using MetuTrade.WebApi.Services;
using MetuTrade.Business.Results;

[ApiController]
public class AdminController : ControllerBase
{
    private readonly BinanceHttpService _binanceService;
    private readonly BarService _barService;
    private readonly BinanceBackgroundDownloadService _binanceBackgroundDownloadService;
    private readonly BinanceBackgroundSocketService _binanceBackgroundSocketService;

    public AdminController(BinanceHttpService binanceService, BarService barService, BinanceBackgroundDownloadService binanceBackgroundDownloadService, BinanceBackgroundSocketService binanceBackgroundSocketService)
    {
        _binanceService = binanceService;
        _barService = barService;
        _binanceBackgroundDownloadService = binanceBackgroundDownloadService;
        _binanceBackgroundSocketService = binanceBackgroundSocketService;
    }

    [HttpPost]
    [Route("/manage/download/start")]
    public async Task<IActionResult> DownloadMarketDataAsync([FromBody] DownloadStartRequestModel model)
    {
        if (string.IsNullOrEmpty(model.Symbol) || string.IsNullOrEmpty(model.Interval))
        {
            return BadRequest();
        }
        _binanceBackgroundDownloadService.StartDownload(model.Symbol, model.Interval, model.StartDate, model.EndDate);
        return Ok();
    }

    [HttpGet]
    [Route("/manage/download/operations")]
    public IActionResult GetDownloadOperations()
    {
        List<BinanceDownloadOperation> operations = _binanceBackgroundDownloadService.GetAllTasks();
        List<DownloadOperationResult> results = new List<DownloadOperationResult>();

        foreach (var op in operations)
        {
            results.Add(new DownloadOperationResult
            {
                CurrentTime = op.CurrentTime,
                EndDate = op.EndDate,
                EndTime = op.EndTime,
                ErrorMessage = op.ErrorMessage,
                Interval = op.Interval,
                StartDate = op.StartDate,
                StartTime = op.StartTime,
                Status = op.Status.ToString(),
                Symbol = op.Symbol,
                TaskId = op.TaskId,
                PackagesReceived = op.PackagesReceived
            });
        }

        return Ok(results);
    }

    [HttpPost]
    [Route("/manage/download/cancel")]
    public IActionResult CancelOperation(Guid taskId)
    {
        bool result = _binanceBackgroundDownloadService.RequestCancel(taskId);
        if (result) return Ok();
        return NotFound();
    }

    [HttpPost]
    [Route("/manage/download/delete-canceled")]
    public IActionResult DeleteCanceledOperations()
    {
        _binanceBackgroundDownloadService.ClearCanceledOperations();
        return Ok();
    }

    [HttpPost]
    [Route("/manage/download/delete-succeeded")]
    public IActionResult DeleteSucceededOperations()
    {
        _binanceBackgroundDownloadService.ClearSucceededOperations();
        return Ok();
    }

    [HttpPost]
    [Route("/manage/download/delete-failed")]
    public IActionResult DeleteFailedOperations()
    {
        _binanceBackgroundDownloadService.ClearFailedOperations();
        return Ok();
    }

    [HttpPost]
    [Route("/manage/subscription/subscribe-klines")]
    public async Task<IActionResult> SubscribeKlinesAsync(SubscribeKlinesRequestModel model)
    {
        await _binanceBackgroundSocketService.SubscribeKlinesAsync(model.Symbol, model.Interval);
        return Ok();
    }

    [HttpPost]
    [Route("/manage/subscription/unsubscribe-klines")]
    public async Task<IActionResult> UnsubscribeKlinesAsync(Guid id)
    {
        bool success = await _binanceBackgroundSocketService.UnsubscribeKlinesAsync(id);
        if (success) return Ok();
        return NotFound();
    }

    [HttpGet]
    [Route("/manage/subscription/list")]
    public IActionResult GetKlineSubscriptionsAsync()
    {
        var list = _binanceBackgroundSocketService.GetAllSubscriptions();
        return Ok(list);
    }
}

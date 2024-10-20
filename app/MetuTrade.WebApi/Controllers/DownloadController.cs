using MetuTrade.Business.RequestModels;
using MetuTrade.Business.Results;
using MetuTrade.WebApi.Services;
using MetuTrade.WebApi.Services.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MetuTrade.WebApi.Controllers
{
    [ApiController]
    public class DownloadController : ControllerBase
    {
        private readonly BinanceBackgroundDownloadService _binanceBackgroundDownloadService;

        public DownloadController(BinanceBackgroundDownloadService binanceBackgroundDownloadService)
        {
            _binanceBackgroundDownloadService = binanceBackgroundDownloadService;
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
        [Route("/manage/download/list")]
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
    }
}

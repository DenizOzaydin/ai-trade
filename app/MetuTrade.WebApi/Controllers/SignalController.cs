using MetuTrade.Business.Results;
using MetuTrade.WebApi.Services;
using MetuTrade.WebApi.Services.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MetuTrade.WebApi.Controllers
{
    [ApiController]
    public class SignalController : ControllerBase
    {
        private readonly SignalGeneratorService _signalGeneratorService;
        
        public SignalController(SignalGeneratorService signalGeneratorService)
        {
            _signalGeneratorService = signalGeneratorService;
        }

        [HttpPost]
        [Route("/manage/signal/start")]
        public async Task<IActionResult> StartBotAsync(int botId, string symbol, string interval)
        {
            bool success = await _signalGeneratorService.StartAsync(botId, symbol, interval);
            if (!success) return NotFound();
            return Ok();
        }

        [HttpPost]
        [Route("/manage/signal/stop")]
        public IActionResult StopBot(Guid id)
        {
            bool success = _signalGeneratorService.Stop(id);
            if (!success) return NotFound();
            return Ok();
        }

        [HttpPost]
        [Route("/manage/signal/list")]
        public IActionResult GetSignalGenerators()
        {
            var entities = _signalGeneratorService.GetAllTasks();
            List<GetSignalGenreatorResult> result = new();
            foreach (var entity in entities)
            {
                result.Add(new GetSignalGenreatorResult
                {
                    Id = entity.Id,
                    Status = entity.Status.ToString(),
                    ErrorMessage = entity.ErrorMessage,
                    Interval = entity.Interval,
                    Symbol = entity.Symbol
                });
            }
            return Ok(result);
        }
    }
}

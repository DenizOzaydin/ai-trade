using MetuTrade.Business.RequestModels;
using MetuTrade.WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MetuTrade.WebApi.Controllers
{
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly BinanceBackgroundSocketService _binanceBackgroundSocketService;

        public SubscriptionController(BinanceBackgroundSocketService binanceBackgroundSocketService)
        {
            _binanceBackgroundSocketService = binanceBackgroundSocketService;
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
}

using MetuTrade.Business.RequestModels;
using MetuTrade.Core.Entities;
using MetuTrade.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MetuTrade.WebApi.Controllers
{
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly BotRepository _botRepository;

        public BotController(BotRepository botRepository)
        {
            _botRepository = botRepository;
        }

        [HttpPost]
        [Route("/manage/bot/upload")]
        public async Task<IActionResult> UploadBot([FromForm] UploadBotRequestModel model)
        {
            string fileName = Guid.NewGuid().ToString() + ".json";
            string botsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "bots");
            string filePath = Path.Combine(botsPath, fileName);

            if (!Directory.Exists(botsPath))
            {
                Directory.CreateDirectory(botsPath);
            }

            using(var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.File.CopyToAsync(stream);
            }

            Bot bot = new Bot
            {
                Name = model.Name,
                Description = model.Description,
                ModelUrl = filePath
            };

            await _botRepository.UpdateAsync(bot);
            await _botRepository.SaveChangesAsync();

            return Ok();
        }
    }
}

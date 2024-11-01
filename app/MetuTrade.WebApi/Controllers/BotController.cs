using MetuTrade.Business.RequestModels;
using MetuTrade.Business.Results;
using MetuTrade.Core.Entities;
using MetuTrade.DataAccess;
using MetuTrade.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

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
        public async Task<IActionResult> UploadBotAsync([FromForm] UploadBotRequestModel model)
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
                Url = filePath
            };

            await _botRepository.UpdateAsync(bot);
            await _botRepository.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        [Route("manage/bot/list")]
        public async Task<IActionResult> GetBotsAsync()
        {
            List<Bot> bots = await _botRepository.GetAllAsync();
            List<GetBotResult> result = new();
            foreach (Bot bot in bots)
            {
                result.Add(new GetBotResult
                {
                    Id = bot.Id,
                    Name = bot.Name,
                    Description = bot.Description,
                    Url = bot.Url,
                });
            }
            return Ok(result);
        }

        [HttpPost]
        [Route("manage/bot/delete")]
        public async Task<IActionResult> DeleteBotAsync(int id)
        {
            Bot? bot = await _botRepository.GetByIdAsync(id);

            if (bot == null) return BadRequest();

            string path = bot.Url;

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            await _botRepository.DeleteAsync(id);
            await _botRepository.SaveChangesAsync();

            return Ok();
        }
    }
}

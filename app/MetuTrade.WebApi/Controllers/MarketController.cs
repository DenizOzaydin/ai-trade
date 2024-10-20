namespace MetuTrade.WebApi.Controllers;

using MetuTrade.Business.Services;
using MetuTrade.Core.Entities;
using MetuTrade.Core.TechnicalAnalysis;
using MetuTrade.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

[ApiController]
public class MarketController : ControllerBase
{
    private readonly BinanceHttpService _binanceService;
    private readonly BarRepository _barRepository;

    public MarketController(BinanceHttpService binanceService, BarRepository barRepository)
    {
        _binanceService = binanceService;
        _barRepository = barRepository;
    }

    [HttpGet]
    [Route("/api/market/get")]
    public async Task<ActionResult<Chart>> GetMarketDataAsync(string symbol, string interval, long? start = null, long? end = null, int entries = 10000000)
    {
        List<Bar> bars = await _barRepository.GetByFilterAsync(e => e.Symbol == symbol && e.Interval == interval && (start == null || e.OpenTime >= start) && (end == null || e.OpenTime <= end), entries);
        Chart chart = new Chart();
        chart.Symbol = symbol;
        chart.Interval = interval;
        chart.Bars = bars;
        return Ok(chart);
    }
}


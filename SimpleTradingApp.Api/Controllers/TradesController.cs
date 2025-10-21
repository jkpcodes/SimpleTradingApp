using Microsoft.AspNetCore.Mvc;
using SimpleTradingApp.Application.DTOs;
using SimpleTradingApp.Application.ServiceContracts;

namespace SimpleTradingApp.Api.Controllers;


[Route("api/[controller]")]
[ApiController]
public class TradesController : ControllerBase
{
    private readonly ITradesService _tradesService;

    public TradesController(ITradesService tradesService)
    {
        _tradesService = tradesService;
    }

    [HttpPost]
    public async Task<ActionResult<TradeResponse?>> AddTrade(AddTradeDto addTradeDto)
    {
        var addedTrade = await _tradesService.AddTrade(addTradeDto);
        if (addedTrade == null)
        {
            return Problem("Unsuccessful in adding trade");
        }

        return Ok(addedTrade);
    }
}

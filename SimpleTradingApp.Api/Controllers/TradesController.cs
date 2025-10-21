using Microsoft.AspNetCore.Mvc;
using SimpleTradingApp.Application.DTOs;
using SimpleTradingApp.Application.ServiceContracts;
using SimpleTradingApp.Domain.Entities;

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

    [HttpPut("{tradeId}/status")]
    public async Task<ActionResult<TradeResponse?>> UpdateTradeStatus(Guid tradeId, UpdateTradeStatusDto updateDto)
    {
        if (tradeId == Guid.Empty || tradeId != updateDto.ID)
        {
            return BadRequest("Trade ID is required and must match the ID in the request body.");
        }
        var updatedTrade = await _tradesService.UpdateTradeStatus(updateDto);

        if (updatedTrade == null)
        {
            return NotFound(new { Message = $"Trade with ID {tradeId} not found."});
        }

        return Ok(updatedTrade);
    }
}

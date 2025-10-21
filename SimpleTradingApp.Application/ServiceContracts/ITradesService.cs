using SimpleTradingApp.Application.DTOs;

namespace SimpleTradingApp.Application.ServiceContracts;

public interface ITradesService
{
    /// <summary>
    /// Adds a new trade
    /// </summary>
    /// <param name="addTradeDto"></param>
    /// <returns>Returns trade response data if successfully added; otherwise null</returns>
    Task<TradeResponse?> AddTrade(AddTradeDto addTradeDto);
}

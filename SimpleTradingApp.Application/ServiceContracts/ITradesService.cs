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

    /// <summary>
    /// Updates the status of an existing trade
    /// </summary>
    /// <param name="updateTradeStatusDto"></param>
    /// <returns>Returns the updated trade response data if successful; otherwise null</returns>
    Task<TradeResponse?> UpdateTradeStatus(UpdateTradeStatusDto updateTradeStatusDto);
}

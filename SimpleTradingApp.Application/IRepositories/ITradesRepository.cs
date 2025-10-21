using SimpleTradingApp.Domain.Entities;

namespace SimpleTradingApp.Application.IRepositories;

public interface ITradesRepository
{
    /// <summary>
    /// Creates a new trade
    /// </summary>
    /// <param name="trade"></param>
    /// <returns>Returns the newly created trade or null if unsuccessful</returns>
    Task<Trade?> AddTrade(Trade trade);

    /// <summary>
    /// Gets a trade by its unique identifier
    /// </summary>
    /// <param name="tradeId"></param>
    /// <returns>Returns the trade identified by ID or null if it doesn't exist</returns>
    Task<Trade?> GetTradeById(Guid tradeId);

    /// <summary>
    /// Updates an existing trade
    /// </summary>
    /// <param name="trade"></param>
    /// <returns>Returns updated trade on success; otherwise return null</returns>
    Task<Trade?> UpdateTrade(Trade trade);
}

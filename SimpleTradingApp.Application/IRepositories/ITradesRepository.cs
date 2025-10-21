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
}

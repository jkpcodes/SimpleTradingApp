

using Microsoft.EntityFrameworkCore;
using SimpleTradingApp.Application.IRepositories;
using SimpleTradingApp.Domain.Entities;

namespace SimpleTradingApp.Infrastructure.Repositories;

public class TradesRepository : ITradesRepository
{
    private readonly AppDbContext _context;

    public TradesRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Trade?> AddTrade(Trade trade)
    {
        trade.Timestamp = DateTime.UtcNow;
        _context.Trades.Add(trade);
        await _context.SaveChangesAsync();
        return trade;
    }

    public async Task<Trade?> GetTradeById(Guid tradeId)
    {
        return await _context.Trades
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.ID == tradeId);
    }

    public async Task<Trade?> UpdateTrade(Trade trade)
    {
        _context.Trades.Update(trade);
        var result = await _context.SaveChangesAsync();

        return result > 0 ? trade : null;
    }
}

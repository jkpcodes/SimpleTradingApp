

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
}

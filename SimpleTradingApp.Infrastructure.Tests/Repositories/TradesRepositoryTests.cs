using Microsoft.EntityFrameworkCore;
using SimpleTradingApp.Domain.Entities;
using SimpleTradingApp.Infrastructure.Repositories;

namespace SimpleTradingApp.Infrastructure.Tests.Repositories;

public class TradesRepositoryTests
{
    private static AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique DB per test
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddTrade_AddsEntityToContext_AndReturnsTrade()
    {
        using var context = GetInMemoryDbContext();
        var repo = new TradesRepository(context);

        var trade = new Trade
        {
            ID = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            SecurityCode = "AAP",
            Amount = 100,
            Type = TradeType.Buy,
            Status = TradeStatus.Placed
        };

        var result = await repo.AddTrade(trade);

        Assert.Same(trade, result);

        var dbTrade = await context.Trades.FirstOrDefaultAsync(t => t.ID == trade.ID);
        Assert.NotNull(dbTrade);
        Assert.Equal(trade.ID, dbTrade.ID);
        Assert.Equal(trade.AccountId, dbTrade.AccountId);
        Assert.Equal(trade.SecurityCode, dbTrade.SecurityCode);
        Assert.Equal(trade.Amount, dbTrade.Amount);
        Assert.Equal(trade.Type, dbTrade.Type);
        Assert.Equal(trade.Status, dbTrade.Status);
    }
}

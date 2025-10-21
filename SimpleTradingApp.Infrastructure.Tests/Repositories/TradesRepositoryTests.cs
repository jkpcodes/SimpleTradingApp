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

    [Fact]
    public async Task GetTradeById_FindNonExistingTrade_ReturnsNull()
    {
        using var context = GetInMemoryDbContext();
        var repo = new TradesRepository(context);

        var result = await repo.GetTradeById(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetTradeById_FindExistingTrade_ReturnsTrade()
    {
        using var context = GetInMemoryDbContext();
        var repo = new TradesRepository(context);
        var trade = new Trade
        {
            ID = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            SecurityCode = "MSFT",
            Amount = 150,
            Type = TradeType.Sell,
            Status = TradeStatus.Placed
        };
        context.Trades.Add(trade);
        await context.SaveChangesAsync();

        var result = await repo.GetTradeById(trade.ID);

        Assert.NotNull(result);
        Assert.Equal(trade.ID, result.ID);
        Assert.Equal(trade.AccountId, result.AccountId);
        Assert.Equal(trade.SecurityCode, result.SecurityCode);
        Assert.Equal(trade.Amount, result.Amount);
        Assert.Equal(trade.Type, result.Type);
        Assert.Equal(trade.Status, result.Status);
    }

    [Fact]
    public async Task UpdateTrade_UpdatesExistingTrade_ReturnsUpdatedTrade()
    {
        using var context = GetInMemoryDbContext();
        var repo = new TradesRepository(context);
        var trade = new Trade
        {
            ID = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            SecurityCode = "ASD",
            Amount = 200,
            Type = TradeType.Buy,
            Status = TradeStatus.Placed
        };
        context.Trades.Add(trade);
        await context.SaveChangesAsync();

        trade.Status = TradeStatus.Executed;
        trade.Amount = 250;

        var result = await repo.UpdateTrade(trade);

        Assert.NotNull(result);
        Assert.Equal(trade.ID, result.ID);
        Assert.Equal(trade.Status, result.Status);
        Assert.Equal(trade.Amount, result.Amount);

        var dbTrade = await context.Trades.FirstOrDefaultAsync(t => t.ID == trade.ID);
        Assert.NotNull(dbTrade);
        Assert.Equal(trade.Status, dbTrade.Status);
        Assert.Equal(trade.Amount, dbTrade.Amount);
    }
}

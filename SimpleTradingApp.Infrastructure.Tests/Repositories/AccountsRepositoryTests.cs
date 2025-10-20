using Microsoft.EntityFrameworkCore;
using SimpleTradingApp.Infrastructure.Repositories;
using SimpleTradingApp.Domain.Entities;

namespace SimpleTradingApp.Infrastructure.Tests.Repositories;

public class AccountsRepositoryTests
{
    private static AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Unique DB per test
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task AddAccount_AddsEntityToContext_AndReturnsAccount()
    {
        using var context = GetInMemoryDbContext();
        var repo = new AccountsRepository(context);

        var account = new Account
        {
            ID = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe"
        };

        var result = await repo.AddAccount(account);

        Assert.Same(account, result);

        // Verify it exists in the database
        var dbAccount = await context.Accounts.FirstOrDefaultAsync(a => a.ID == account.ID);
        Assert.NotNull(dbAccount);
        Assert.Same(account, dbAccount);
    }

    [Fact]
    public async Task DeleteAccount_RemovesAccountAndTrades_WhenAccountExists()
    {
        using var context = GetInMemoryDbContext();
        var repo = new AccountsRepository(context);

        var account = new Account
        {
            ID = Guid.NewGuid(),
            FirstName = "Jane",
            LastName = "Doe",
        };

        var trade1 = new Trade
        {
            ID = Guid.NewGuid(),
            AccountId = account.ID,
            SecurityCode = "AAP",
            Timestamp = DateTime.UtcNow,
            Amount = 100,
            Type = TradeType.Buy,
            Status = TradeStatus.Placed
        };

        var trade2 = new Trade
        {
            ID = Guid.NewGuid(),
            AccountId = account.ID,
            SecurityCode = "ZXZ",
            Timestamp = DateTime.UtcNow,
            Amount = 123,
            Type = TradeType.Sell,
            Status = TradeStatus.Executed
        };

        context.Accounts.Add(account);
        context.Trades.AddRange(trade1, trade2);
        await context.SaveChangesAsync();

        var result = await repo.DeleteAccount(account.ID);

        Assert.True(result);
        Assert.False(context.Accounts.Any(a => a.ID == account.ID));
        Assert.False(context.Trades.Any(t => t.AccountId == account.ID));
    }

    [Fact]
    public async Task DeleteAccount_ReturnsFalse_WhenAccountDoesNotExist()
    {
        using var context = GetInMemoryDbContext();
        var repo = new AccountsRepository(context);

        var result = await repo.DeleteAccount(Guid.NewGuid());

        Assert.False(result);
    }
}

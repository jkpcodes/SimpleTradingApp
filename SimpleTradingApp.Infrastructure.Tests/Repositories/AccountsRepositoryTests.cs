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

    [Fact]
    public async Task GetAccountById_ReturnsAccount_WhenItExists()
    {
        using var context = GetInMemoryDbContext();
        var repo = new AccountsRepository(context);
        var account = new Account
        {
            ID = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "User"
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

        var result = await repo.GetAccountById(account.ID, true);

        Assert.NotNull(result);
        Assert.Equal(account.ID, result.ID);
        Assert.Equal(account.FirstName, result.FirstName);
        Assert.Equal(account.LastName, result.LastName);
        Assert.Equal(account.Trades.Count, 2);
    }

    [Fact]
    public async Task GetAccountById_ReturnsAccountWithTrades_WhenItExists()
    {
        using var context = GetInMemoryDbContext();
        var repo = new AccountsRepository(context);
        var account = new Account
        {
            ID = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "User"
        };
        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        var result = await repo.GetAccountById(account.ID, true);

        Assert.NotNull(result);
        Assert.Equal(account.ID, result.ID);
        Assert.Equal(account.FirstName, result.FirstName);
        Assert.Equal(account.LastName, result.LastName);
    }

    [Fact]
    public async Task GetAccountById_ReturnsNull_WhenAccountDoesNotExist()
    {
        using var context = GetInMemoryDbContext();
        var repo = new AccountsRepository(context);

        var result = await repo.GetAccountById(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAccount_UpdatesEntityInContext_AndReturnsAccount()
    {
        using var context = GetInMemoryDbContext();
        var repo = new AccountsRepository(context);
        var account = new Account
        {
            ID = Guid.NewGuid(),
            FirstName = "Initial",
            LastName = "Name"
        };
        context.Accounts.Add(account);
        await context.SaveChangesAsync();

        // Modify account
        account.FirstName = "Updated";
        account.LastName = "Name";

        var result = await repo.UpdateAccount(account);

        Assert.Same(account, result);
        var dbAccount = await context.Accounts.FirstOrDefaultAsync(a => a.ID == account.ID);
        Assert.NotNull(dbAccount);
        Assert.Equal("Updated", dbAccount.FirstName);
        Assert.Equal("Name", dbAccount.LastName);
    }
}

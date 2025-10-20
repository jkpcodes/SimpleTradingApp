using Microsoft.EntityFrameworkCore;
using SimpleTradingApp.Infrastructure.Repositories;
using SimpleTradingApp.Domain.Entities;
using SimpleTradingApp.Application.DTOs;

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

    [Fact]
    public async Task GetAccounts_PagingParamsInRange_ReturnsPagedItemsAndCountWithOrdering()
    {
        using var context = GetInMemoryDbContext();
        // Add 3 accounts with different last/first names to test ordering
        var accounts = new List<Account>
        {
            new Account { ID = Guid.NewGuid(), FirstName = "John", LastName = "Doe" },
            new Account { ID = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith" },
            new Account { ID = Guid.NewGuid(), FirstName = "Alice", LastName = "Brown" }
        };

        context.Accounts.AddRange(accounts);
        await context.SaveChangesAsync();

        var repo = new AccountsRepository(context);

        // pageNumber = 1, pageSize = 2  -> should return first 2 accounts ordered by LastName, FirstName
        var (itemsPage1, totalCount1) = await repo.GetAccounts(
            new PagingParameters() { PageNumber = 1, PageSize = 2 });

        Assert.Equal(3, totalCount1);
        var page1List = itemsPage1.ToList();
        Assert.Equal(2, page1List.Count);

        // Verify ordering: 1st should be Alice Brown, 2nd John Doe
        Assert.Equal("Brown", page1List[0].LastName);
        Assert.Equal("Alice", page1List[0].FirstName);
        Assert.Equal("Doe", page1List[1].LastName);
        Assert.Equal("John", page1List[1].FirstName);

        // pageNumber = 2, pageSize = 2 -> should return last account
        var (itemsPage2, totalCount2) = await repo.GetAccounts(
            new PagingParameters() { PageNumber = 2, PageSize = 2 });

        Assert.Equal(3, totalCount2);
        var page2List = itemsPage2.ToList();
        Assert.Single(page2List);
        Assert.Equal("Smith", page2List[0].LastName);
        Assert.Equal("Jane", page2List[0].FirstName);
    }

    [Fact]
    public async Task GetAccounts_PagingParamsOutOfRange_ReturnsEmptyItems()
    {
        using var context = GetInMemoryDbContext();
        var accounts = new List<Account>
        {
            new Account { ID = Guid.NewGuid(), FirstName = "John", LastName = "Doe" },
            new Account { ID = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith" }
        };

        context.Accounts.AddRange(accounts);
        await context.SaveChangesAsync();

        var repo = new AccountsRepository(context);

        // PageNumber = 3, PageSize = 2 -> only 1 page exists, so should return empty collection
        var (items, totalCount) = await repo.GetAccounts(
            new PagingParameters() { PageNumber = 3, PageSize = 2 });

        Assert.Equal(2, totalCount);
        Assert.Empty(items);
    }

    [Fact]
    public async Task SearchAccounts_ByFullId_ReturnsOnlyMatchingAccount()
    {
        using var context = GetInMemoryDbContext();
        var repo = new AccountsRepository(context);

        var a1 = new Account { ID = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
        var a2 = new Account { ID = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith" };
        context.Accounts.AddRange(a1, a2);
        await context.SaveChangesAsync();

        var (items, totalCount) = await repo.SearchAccounts(
            new SearchPagingParameters()
            {
                ID = a1.ID.ToString(),
                PageNumber = 1,
                PageSize = 10
            });

        Assert.Equal(1, totalCount);
        var list = items.ToList();
        Assert.Single(list);
        Assert.Equal(a1.ID, list[0].ID);
    }

    [Fact]
    public async Task SearchAccounts_ByPartialId_ReturnsOnlyMatchingAccount()
    {
        using var context = GetInMemoryDbContext();
        var repo = new AccountsRepository(context);

        var a1 = new Account { ID = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
        var a2 = new Account { ID = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith" };
        context.Accounts.AddRange(a1, a2);
        await context.SaveChangesAsync();

        // take first 8 chars of GUID to act as partial filter
        var partialId = a1.ID.ToString().Substring(0, 8);

        var (items, totalCount) = await repo.SearchAccounts(
            new SearchPagingParameters()
            {
                ID = partialId,
                PageNumber = 1,
                PageSize = 10
            });

        Assert.Equal(1, totalCount);
        var list = items.ToList();
        Assert.Single(list);
        Assert.Equal(a1.ID, list[0].ID);
    }

    [Fact]
    public async Task SearchAccounts_ByLastName_ReturnsOnlyMatchingAccount()
    {
        using var context = GetInMemoryDbContext();
        var repo = new AccountsRepository(context);

        var a1 = new Account { ID = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
        var a2 = new Account { ID = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith" };
        context.Accounts.AddRange(a1, a2);
        await context.SaveChangesAsync();

        var (items, totalCount) = await repo.SearchAccounts(
            new SearchPagingParameters()
            {
                LastName = "DO",
                PageNumber = 1,
                PageSize = 10
            });

        Assert.Equal(1, totalCount);
        var list = items.ToList();
        Assert.Single(list);
        Assert.Equal(a1.ID, list[0].ID);
    }

    [Fact]
    public async Task SearchAccounts_UsingBothFilters_ReturnsMatchingAccount()
    {
        using var context = GetInMemoryDbContext();
        var repo = new AccountsRepository(context);

        var a1 = new Account { ID = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
        var a2 = new Account { ID = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith" };
        context.Accounts.AddRange(a1, a2);
        await context.SaveChangesAsync();

        var (items, totalCount) = await repo.SearchAccounts(
            new SearchPagingParameters()
            {
                ID = a1.ID.ToString().Substring(0, 8),
                LastName = "Doe",
                PageNumber = 1,
                PageSize = 10
            });

        Assert.Equal(1, totalCount);
        var list = items.ToList();
        Assert.Single(list);
        Assert.Equal(a1.ID, list[0].ID);
    }

    [Fact]
    public async Task SearchAccounts_UsingBothFilters_ReturnsNoMatchingAccount()
    {
        using var context = GetInMemoryDbContext();
        var repo = new AccountsRepository(context);

        var a1 = new Account { ID = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
        var a2 = new Account { ID = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith" };
        context.Accounts.AddRange(a1, a2);
        await context.SaveChangesAsync();

        var (items, totalCount) = await repo.SearchAccounts(
            new SearchPagingParameters()
            {
                ID = a1.ID.ToString().Substring(0, 8),
                LastName = "NotExist",
                PageNumber = 1,
                PageSize = 10
            });

        Assert.Equal(0, totalCount);
        Assert.Empty(items);
    }
}

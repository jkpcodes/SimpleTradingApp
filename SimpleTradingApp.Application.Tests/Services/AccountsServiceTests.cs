using Moq;
using SimpleTradingApp.Application.DTOs;
using SimpleTradingApp.Application.IRepositories;
using SimpleTradingApp.Application.Services;
using SimpleTradingApp.Domain.Entities;

namespace SimpleTradingApp.Application.Tests.Services;

public class AccountsServiceTests
{
    [Fact]
    public async Task AddAccount_ReturnsNull_WhenRepositoryReturnsNull()
    {
        var repo = new Mock<IAccountsRepository>();
        repo.Setup(r => r.AddAccount(It.IsAny<Account>()))
            .ReturnsAsync((Account?)null);
        var service = new AccountsService(repo.Object);

        var result = await service.AddAccount(new AddAccountDto("John", "Doe"));

        Assert.Null(result);
    }

    [Fact]
    public async Task AddAccount_ReturnsAccountResponse_WhenRepositoryReturnsAccount()
    {
        var account = new Account
        {
            ID = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe"
        };

        var repo = new Mock<IAccountsRepository>();
        repo.Setup(r => r.AddAccount(It.IsAny<Account>()))
            .ReturnsAsync(account);
        var service = new AccountsService(repo.Object);

        var result = await service.AddAccount(new AddAccountDto("John", "Doe"));

        Assert.NotNull(result);
        Assert.Equal(account.ID, result!.ID);
        Assert.Equal(account.FirstName, result.FirstName);
        Assert.Equal(account.LastName, result.LastName);

        repo.Verify(r => r.AddAccount(It.Is<Account>(a => a.FirstName == "John" && a.LastName == "Doe")),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAccount_NonExistentAccount_AndReturnsFalse()
    {
        var repo = new Mock<IAccountsRepository>();
        var accountId = Guid.NewGuid();
        repo.Setup(r => r.GetAccountById(accountId, false))
            .ReturnsAsync((Account?)null);

        var service = new AccountsService(repo.Object);

        var result = await service.DeleteAccount(accountId);

        Assert.False(result);
        repo.Verify(r => r.GetAccountById(accountId, false), Times.Once);
        repo.Verify(r => r.DeleteAccount(accountId), Times.Never);
    }

    [Fact]
    public async Task DeleteAccount_CallsRepository_AndReturnsTrue()
    {
        var repo = new Mock<IAccountsRepository>();
        var accountId = Guid.NewGuid();
        repo.Setup(r => r.GetAccountById(accountId, false))
            .ReturnsAsync(new Account { ID = accountId, FirstName = "Test", LastName = "User" });
        repo.Setup(r => r.DeleteAccount(accountId))
            .ReturnsAsync(true);

        var service = new AccountsService(repo.Object);

        var result = await service.DeleteAccount(accountId);

        Assert.True(result);
        repo.Verify(r => r.GetAccountById(accountId, false), Times.Once);
        repo.Verify(r => r.DeleteAccount(accountId), Times.Once);
    }

    [Fact]
    public async Task DeleteAccount_CallsRepository_AndReturnsFalse()
    {
        var repo = new Mock<IAccountsRepository>();
        var accountId = Guid.NewGuid();
        repo.Setup(r => r.GetAccountById(accountId, false))
            .ReturnsAsync(new Account { ID = accountId, FirstName = "Test", LastName = "User" });
        repo.Setup(r => r.DeleteAccount(accountId))
            .ReturnsAsync(false);

        var service = new AccountsService(repo.Object);

        var result = await service.DeleteAccount(accountId);

        Assert.False(result);
        repo.Verify(r => r.GetAccountById(accountId, false), Times.Once);
        repo.Verify(r => r.DeleteAccount(accountId), Times.Once);
    }

    [Fact]
    public async Task GetAccountById_NonExistentAccount_AndReturnsNull()
    {
        var repo = new Mock<IAccountsRepository>();
        var accountId = Guid.NewGuid();
        repo.Setup(r => r.GetAccountById(accountId, true))
            .ReturnsAsync((Account?)null);

        var service = new AccountsService(repo.Object);

        var result = await service.GetAccountById(accountId);

        Assert.Null(result);
        repo.Verify(r => r.GetAccountById(accountId, true), Times.Once);
    }

    [Fact]
    public async Task GetAccountById_ExistingAccount_AndReturnsAccountData()
    {
        var accountId = Guid.NewGuid();
        var account = new Account
        {
            ID = accountId,
            FirstName = "John",
            LastName = "Doe"
        };
        var repo = new Mock<IAccountsRepository>();
        repo.Setup(r => r.GetAccountById(accountId, true))
            .ReturnsAsync(account);

        var service = new AccountsService(repo.Object);

        var result = await service.GetAccountById(accountId);

        Assert.NotNull(result);
        Assert.Equal(account.ID, result.ID);
        Assert.Equal(account.FirstName, result.FirstName);
        Assert.Equal(account.LastName, result.LastName);
        Assert.Equal(account.Trades, result.Trades);
        repo.Verify(r => r.GetAccountById(accountId, true), Times.Once);
    }

    [Fact]
    public async Task UpdateAccount_NonExistentAccount_AndReturnsNull()
    {
        var repo = new Mock<IAccountsRepository>();
        var accountId = Guid.NewGuid();
        repo.Setup(r => r.GetAccountById(accountId, false))
            .ReturnsAsync((Account?)null);

        var service = new AccountsService(repo.Object);

        var result = await service.UpdateAccount(new UpdateAccountDto(accountId, "Jane", "Doe"));

        Assert.Null(result);
        repo.Verify(r => r.GetAccountById(accountId, false), Times.Once);
        repo.Verify(r => r.UpdateAccount(It.IsAny<Account>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAccount_ExistingAccount_AndReturnsAccountResponse()
    {
        var existingAccount = new Account
        {
            ID = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe"
        };
        var updatedAccount = new Account
        {
            ID = existingAccount.ID,
            FirstName = "Jane",
            LastName = "Smith"
        };
        var repo = new Mock<IAccountsRepository>();
        repo.Setup(r => r.GetAccountById(existingAccount.ID, false))
            .ReturnsAsync(existingAccount);
        repo.Setup(r => r.UpdateAccount(It.IsAny<Account>()))
            .ReturnsAsync(updatedAccount);

        var service = new AccountsService(repo.Object);

        var result = await service.UpdateAccount(new UpdateAccountDto(existingAccount.ID, "Jane", "Smith"));

        Assert.NotNull(result);
        Assert.Equal(updatedAccount.ID, result.ID);
        Assert.Equal(updatedAccount.FirstName, result.FirstName);
        Assert.Equal(updatedAccount.LastName, result.LastName);
        repo.Verify(r => r.GetAccountById(existingAccount.ID, false), Times.Once);
        repo.Verify(r => r.UpdateAccount(
            It.Is<Account>(a => a.ID == existingAccount.ID && a.FirstName == "Jane" && a.LastName == "Smith")),
            Times.Once);
    }

    [Fact]
    public async Task GetAccounts_InvalidPaginParameters_AndReturnsDefaultPaginatedResponse()
    {
        var repo = new Mock<IAccountsRepository>();
        var accounts = new List<Account>
        {
            new Account { ID = Guid.NewGuid(), FirstName = "John", LastName = "Doe" },
            new Account { ID = Guid.NewGuid(), FirstName = "Jane", LastName = "Smith" }
        };

        repo.Setup(r => r.GetAccounts(It.Is<PagingParameters>(p => p.PageNumber == 1 && p.PageSize == 10)))
            .ReturnsAsync((accounts.AsEnumerable(), accounts.Count));
        var service = new AccountsService(repo.Object);

        var result = await service.GetAccounts(new PagingParameters() { PageNumber = 0, PageSize = 0 });

        Assert.NotNull(result);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(2, result.TotalItems);
        Assert.Equal(1, result.TotalPages);
        repo.Verify(r => r.GetAccounts(It.Is<PagingParameters>(p => p.PageNumber == 1 && p.PageSize == 10)), Times.Once);
    }

    [Fact]
    public async Task GetAccounts_MapsAccountsToResponses_AndReturnsCorrectPaginationMetadata()
    {
        var repo = new Mock<IAccountsRepository>();

        var accounts = new List<Account>
        {
            new Account { ID = Guid.NewGuid(), FirstName = "John", LastName = "Alpha" },
            new Account { ID = Guid.NewGuid(), FirstName = "Jane", LastName = "Beta" }
        };

        // Suppose there are 5 items, pageSize = 2 -> totalPages = 3
        repo.Setup(r => r.GetAccounts(It.Is<PagingParameters>(p => p.PageNumber == 2 && p.PageSize == 2)))
            .ReturnsAsync((accounts.AsEnumerable(), 5));

        var service = new AccountsService(repo.Object);

        var result = await service.GetAccounts(new PagingParameters { PageNumber = 2, PageSize = 2 });

        Assert.NotNull(result);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(2, result.PageSize);
        Assert.Equal(5, result.TotalItems);
        Assert.Equal(3, result.TotalPages);

        var items = result.Items.ToList();
        Assert.Equal(2, items.Count);
        Assert.Contains(items, i => i.FirstName == "John" && i.LastName == "Alpha");
        Assert.Contains(items, i => i.FirstName == "Jane" && i.LastName == "Beta");

        repo.Verify(r => r.GetAccounts(It.Is<PagingParameters>(p => p.PageNumber == 2 && p.PageSize == 2)), Times.Once);
    }
}

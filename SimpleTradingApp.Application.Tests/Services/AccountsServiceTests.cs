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
    public async Task DeleteAccount_CallsRepository_AndReturnsTrue()
    {
        var repo = new Mock<IAccountsRepository>();
        var accountId = Guid.NewGuid();
        repo.Setup(r => r.DeleteAccount(accountId))
            .ReturnsAsync(true);

        var service = new AccountsService(repo.Object);

        var result = await service.DeleteAccount(accountId);

        Assert.True(result);
        repo.Verify(r => r.DeleteAccount(accountId), Times.Once);
    }

    [Fact]
    public async Task DeleteAccount_CallsRepository_AndReturnsFalse()
    {
        var repo = new Mock<IAccountsRepository>();
        var accountId = Guid.NewGuid();
        repo.Setup(r => r.DeleteAccount(accountId))
            .ReturnsAsync(false);

        var service = new AccountsService(repo.Object);

        var result = await service.DeleteAccount(accountId);

        Assert.False(result);
        repo.Verify(r => r.DeleteAccount(accountId), Times.Once);
    }
}

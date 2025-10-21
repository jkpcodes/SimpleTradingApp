using Moq;
using SimpleTradingApp.Application.DTOs;
using SimpleTradingApp.Application.IRepositories;
using SimpleTradingApp.Application.Services;
using SimpleTradingApp.Domain.Entities;

namespace SimpleTradingApp.Application.Tests.Services;

public class TradesServiceTests
{
    [Fact]
    public async Task AddTrade_ReturnsNull_WhenAccountDoesNotExist()
    {
        var tradesRepo = new Mock<ITradesRepository>();
        var accountsRepo = new Mock<IAccountsRepository>();
        accountsRepo.Setup(s => s.GetAccountById(It.IsAny<Guid>(), false))
            .ReturnsAsync((Account?)null);

        var service = new TradesService(tradesRepo.Object, accountsRepo.Object);

        var dto = new AddTradeDto(Guid.NewGuid(), "ABC", 100m, TradeType.Buy);

        var result = await service.AddTrade(dto);

        Assert.Null(result);
        tradesRepo.Verify(r => r.AddTrade(It.IsAny<Trade>()), Times.Never);
    }

    [Fact]
    public async Task AddTrade_ReturnsNull_WhenAddTradeReturnsNull()
    {
        var tradesRepo = new Mock<ITradesRepository>();
        tradesRepo.Setup(r => r.AddTrade(It.IsAny<Trade>()))
                  .ReturnsAsync((Trade?)null);

        var accountsRepo = new Mock<IAccountsRepository>();
        var account = new Account
        {
            ID = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe"
        };
        accountsRepo.Setup(s => s.GetAccountById(It.IsAny<Guid>(), false))
            .ReturnsAsync(account);

        var service = new TradesService(tradesRepo.Object, accountsRepo.Object);

        var dto = new AddTradeDto(account.ID, "ABC", 50m, TradeType.Sell);

        var result = await service.AddTrade(dto);

        Assert.Null(result);
        tradesRepo.Verify(r => r.AddTrade(It.IsAny<Trade>()), Times.Once);
    }

    [Fact]
    public async Task AddTrade_ReturnsTradeResponse_WhenAddTradeReturnsTradeObject()
    {
        var tradesRepo = new Mock<ITradesRepository>();

        var dto = new AddTradeDto(Guid.NewGuid(), "XYZ", 250.75m, TradeType.Buy);
        var returnedTrade = new Trade
        {
            ID = Guid.NewGuid(),
            AccountId = dto.AccountId,
            SecurityCode = "XYZ",
            Timestamp = DateTime.UtcNow,
            Amount = 250.75m,
            Type = TradeType.Buy,
            Status = TradeStatus.Placed
        };

        tradesRepo.Setup(r => r.AddTrade(It.IsAny<Trade>()))
            .ReturnsAsync(returnedTrade);

        var accountsRepo = new Mock<IAccountsRepository>();
        var account = new Account
        {
            ID = dto.AccountId,
            FirstName = "John",
            LastName = "Doe"
        };
        accountsRepo.Setup(s => s.GetAccountById(dto.AccountId, false))
            .ReturnsAsync(account);

        var service = new TradesService(tradesRepo.Object, accountsRepo.Object);

        var result = await service.AddTrade(dto);

        Assert.NotNull(result);
        Assert.Equal(returnedTrade.ID, result.ID);
        Assert.Equal(returnedTrade.AccountId, result.AccountId);
        Assert.Equal(returnedTrade.SecurityCode, result.SecurityCode);
        Assert.Equal(returnedTrade.Amount, result.Amount);
        Assert.Equal(returnedTrade.Type, result.Type);
        Assert.Equal(returnedTrade.Status, result.Status);

        tradesRepo.Verify(r => r.AddTrade(It.Is<Trade>(t =>
            t.AccountId == dto.AccountId &&
            t.SecurityCode == dto.SecurityCode &&
            t.Amount == dto.Amount &&
            t.Type == dto.Type
        )), Times.Once);
    }
}

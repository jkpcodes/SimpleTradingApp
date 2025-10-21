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

    [Fact]
    public async Task UpdateTradeStatus_ReturnsNull_WhenGetTradeByIdReturnsNull()
    {
        var tradesRepo = new Mock<ITradesRepository>();
        tradesRepo.Setup(r => r.GetTradeById(It.IsAny<Guid>()))
            .ReturnsAsync((Trade?)null);

        var accountsRepo = new Mock<IAccountsRepository>();
        var service = new TradesService(tradesRepo.Object, accountsRepo.Object);

        var dto = new UpdateTradeStatusDto(Guid.NewGuid(), TradeStatus.Executed);

        var result = await service.UpdateTradeStatus(dto);

        Assert.Null(result);
        tradesRepo.Verify(r => r.GetTradeById(dto.ID), Times.Once);
        tradesRepo.Verify(r => r.UpdateTrade(It.IsAny<Trade>()), Times.Never);
    }

    [Fact]
    public async Task UpdateTradeStatus_ReturnsNull_WhenUpdateTradeReturnsNull()
    {
        var existingTrade = new Trade
        {
            ID = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            SecurityCode = "LMN",
            Amount = 150m,
            Type = TradeType.Sell,
            Status = TradeStatus.Placed
        };
        var tradesRepo = new Mock<ITradesRepository>();
        tradesRepo.Setup(r => r.GetTradeById(existingTrade.ID))
            .ReturnsAsync(existingTrade);
        tradesRepo.Setup(r => r.UpdateTrade(It.IsAny<Trade>()))
            .ReturnsAsync((Trade?)null);

        var accountsRepo = new Mock<IAccountsRepository>();
        var service = new TradesService(tradesRepo.Object, accountsRepo.Object);

        var dto = new UpdateTradeStatusDto(existingTrade.ID, TradeStatus.Executed);

        var result = await service.UpdateTradeStatus(dto);

        Assert.Null(result);

        tradesRepo.Verify(r => r.GetTradeById(dto.ID), Times.Once);
        tradesRepo.Verify(r => r.UpdateTrade(It.Is<Trade>(t =>
            t.ID == existingTrade.ID &&
            t.Status == dto.Status
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateTradeStatus_ReturnsTradeResponse_WhenUpdateSucceeds()
    {
        var tradesRepo = new Mock<ITradesRepository>();
        var existingTrade = new Trade
        {
            ID = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            SecurityCode = "AAA",
            Timestamp = DateTime.UtcNow,
            Amount = 10,
            Type = TradeType.Buy,
            Status = TradeStatus.Placed
        };

        var updatedTrade = new Trade
        {
            ID = existingTrade.ID,
            AccountId = existingTrade.AccountId,
            SecurityCode = existingTrade.SecurityCode,
            Timestamp = existingTrade.Timestamp,
            Amount = existingTrade.Amount,
            Type = existingTrade.Type,
            Status = TradeStatus.Executed
        };

        tradesRepo.Setup(r => r.GetTradeById(existingTrade.ID))
                  .ReturnsAsync(existingTrade);

        tradesRepo.Setup(r => r.UpdateTrade(It.Is<Trade>(t => t.ID == existingTrade.ID && t.Status == TradeStatus.Executed)))
                  .ReturnsAsync(updatedTrade);

        var accountsRepo = new Mock<IAccountsRepository>();
        var service = new TradesService(tradesRepo.Object, accountsRepo.Object);

        var dto = new UpdateTradeStatusDto(existingTrade.ID, TradeStatus.Executed);

        var result = await service.UpdateTradeStatus(dto);

        Assert.NotNull(result);
        Assert.Equal(updatedTrade.ID, result.ID);
        Assert.Equal(updatedTrade.Status, result.Status);

        tradesRepo.Verify(r => r.GetTradeById(dto.ID), Times.Once);
        tradesRepo.Verify(r => r.UpdateTrade(It.Is<Trade>(t => t.Status == TradeStatus.Executed)), Times.Once);
    }
}

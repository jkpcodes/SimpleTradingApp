using Microsoft.AspNetCore.Mvc;
using Moq;
using SimpleTradingApp.Api.Controllers;
using SimpleTradingApp.Application.DTOs;
using SimpleTradingApp.Application.ServiceContracts;
using SimpleTradingApp.Domain.Entities;
using System.Net;

namespace SimpleTradingApp.Api.Tests.Controllers;

public class TradesControllerTests
{
    [Fact]
    public async Task AddTrade_ReturnsProblem_WhenServiceReturnsNull()
    {
        var svc = new Mock<ITradesService>();
        svc.Setup(s => s.AddTrade(It.IsAny<AddTradeDto>()))
           .ReturnsAsync((TradeResponse?)null);

        var controller = new TradesController(svc.Object);

        var result = await controller.AddTrade(new AddTradeDto(Guid.NewGuid(), "ABC", 10, TradeType.Buy));

        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
    }

    [Fact]
    public async Task AddTrade_ReturnsOk_WhenServiceReturnsTradeResponse()
    {
        var svc = new Mock<ITradesService>();
        var tradeResponse = new TradeResponse(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "XYZ",
            DateTime.UtcNow,
            200,
            TradeType.Sell,
            TradeStatus.Placed
        );

        svc.Setup(s => s.AddTrade(It.IsAny<AddTradeDto>()))
           .ReturnsAsync(tradeResponse);

        var controller = new TradesController(svc.Object);

        var result = await controller.AddTrade(new AddTradeDto(tradeResponse.AccountId, "XYZ", 200m, TradeType.Sell));

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        Assert.Equal(tradeResponse, okResult.Value);
    }
}

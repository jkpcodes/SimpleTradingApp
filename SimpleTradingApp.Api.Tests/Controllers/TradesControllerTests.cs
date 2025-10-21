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

    [Fact]
    public async Task UpdateTradeStatus_ReturnsBadRequest_WhenTradeIdIsEmpty()
    {
        var svc = new Mock<ITradesService>();
        var controller = new TradesController(svc.Object);
        
        var dto = new UpdateTradeStatusDto(Guid.NewGuid(), TradeStatus.Executed);

        var result = await controller.UpdateTradeStatus(Guid.Empty, dto);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        svc.Verify(svc => svc.UpdateTradeStatus(It.IsAny<UpdateTradeStatusDto>()), Times.Never);
    }

    [Fact]
    public async Task UpdateTradeStatus_ReturnsBadRequest_WhenTradeIdDoesNotMatchDtoId()
    {
        var svc = new Mock<ITradesService>();
        var controller = new TradesController(svc.Object);

        var dto = new UpdateTradeStatusDto(Guid.NewGuid(), TradeStatus.Executed);

        var result = await controller.UpdateTradeStatus(Guid.NewGuid(), dto);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        svc.Verify(svc => svc.UpdateTradeStatus(It.IsAny<UpdateTradeStatusDto>()), Times.Never);
    }

    [Fact]
    public async Task UpdateTradeStatus_ReturnsNotFound_WhenServiceReturnsNull()
    {
        var svc = new Mock<ITradesService>();
        svc.Setup(s => s.UpdateTradeStatus(It.IsAny<UpdateTradeStatusDto>()))
           .ReturnsAsync((TradeResponse?)null);
        var controller = new TradesController(svc.Object);

        var tradeId = Guid.NewGuid();
        var dto = new UpdateTradeStatusDto(tradeId, TradeStatus.Executed);

        var result = await controller.UpdateTradeStatus(tradeId, dto);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task UpdateTradeStatus_ReturnsOk_WhenServiceReturnsTradeResponse()
    {
        var svc = new Mock<ITradesService>();
        var tradeResponse = new TradeResponse(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "XYZ",
            DateTime.UtcNow,
            150,
            TradeType.Buy,
            TradeStatus.Executed
        );
        svc.Setup(s => s.UpdateTradeStatus(It.IsAny<UpdateTradeStatusDto>()))
           .ReturnsAsync(tradeResponse);
        var controller = new TradesController(svc.Object);

        var dto = new UpdateTradeStatusDto(tradeResponse.ID, TradeStatus.Executed);

        var result = await controller.UpdateTradeStatus(tradeResponse.ID, dto);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        Assert.Equal(tradeResponse, okResult.Value);
    }
}

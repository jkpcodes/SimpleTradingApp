using Microsoft.AspNetCore.Mvc;
using Moq;
using SimpleTradingApp.Api.Controllers;
using SimpleTradingApp.Application.DTOs;
using SimpleTradingApp.Application.ServiceContracts;
using System.Net;
using Xunit;

namespace SimpleTradingApp.Api.Tests.Controllers;

public class AccountsControllerTests
{
    [Fact]
    public async Task AddAccount_ReturnsProblem_WhenServiceReturnsNull()
    {
        var svc = new Mock<IAccountsService>();
        svc.Setup(s => s.AddAccount(It.IsAny<AddAccountDto>()))
           .ReturnsAsync((AccountResponse?)null);
        var controller = new AccountsController(svc.Object);

        var result = await controller.AddAccount(new AddAccountDto("John", "Doe"));

        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.InternalServerError, objectResult.StatusCode);
    }

    [Fact]
    public async Task AddAccount_ReturnsOk_WhenServiceReturnsAccountResponse()
    {
        var svc = new Mock<IAccountsService>();
        var accountResponse = new AccountResponse(Guid.NewGuid(), "John", "Doe", []);
        svc.Setup(s => s.AddAccount(It.IsAny<AddAccountDto>()))
           .ReturnsAsync(accountResponse);

        var controller = new AccountsController(svc.Object);
        var result = await controller.AddAccount(new AddAccountDto("John", "Doe"));

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        Assert.Equal(accountResponse, okResult.Value);
    }

    [Fact]
    public async Task DeleteAccount_ReturnsBadRequest_WhenAccountIdIsEmpty()
    {
        var svc = new Mock<IAccountsService>();
        var controller = new AccountsController(svc.Object);

        var result = await controller.DeleteAccount(Guid.Empty);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        svc.Verify(svc => svc.DeleteAccount(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAccount_ReturnsNotFound_WhenServiceReturnsFalse()
    {
        var svc = new Mock<IAccountsService>();
        var id = Guid.NewGuid();
        svc.Setup(s => s.DeleteAccount(id))
           .ReturnsAsync(false);

        var controller = new AccountsController(svc.Object);

        var result = await controller.DeleteAccount(id);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
        svc.Verify(svc => svc.DeleteAccount(id), Times.Once);
    }

    [Fact]
    public async Task DeleteAccount_ReturnsOk_WhenServiceReturnsTrue()
    {
        var svc = new Mock<IAccountsService>();
        var id = Guid.NewGuid();
        svc.Setup(s => s.DeleteAccount(id))
           .ReturnsAsync(true);

        var controller = new AccountsController(svc.Object);

        var result = await controller.DeleteAccount(id);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        Assert.Equal(true, okResult.Value);

        svc.Verify(svc => svc.DeleteAccount(id), Times.Once);
    }
}

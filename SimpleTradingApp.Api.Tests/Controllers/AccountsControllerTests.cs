using Microsoft.AspNetCore.Mvc;
using Moq;
using SimpleTradingApp.Api.Controllers;
using SimpleTradingApp.Application.DTOs;
using SimpleTradingApp.Application.ServiceContracts;
using SimpleTradingApp.Domain.Entities;
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

    [Fact]
    public async Task GetAccountByID_ReturnsBadRequest_WhenAccountIdIsEmpty()
    {
        var svc = new Mock<IAccountsService>();
        var controller = new AccountsController(svc.Object);

        var result = await controller.GetAccountById(Guid.Empty);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        svc.Verify(svc => svc.GetAccountById(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task GetAccountByID_ReturnsNotFound_WhenServiceReturnsNull()
    {
        var svc = new Mock<IAccountsService>();
        var accountId = Guid.NewGuid();
        svc.Setup(s => s.GetAccountById(accountId))
           .ReturnsAsync((AccountResponse?)null);
        var controller = new AccountsController(svc.Object);

        var result = await controller.GetAccountById(accountId);

        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.NotFound, notFoundResult.StatusCode);
        svc.Verify(svc => svc.GetAccountById(accountId), Times.Once);
    }

    [Fact]
    public async Task GetAccountByID_ReturnsOk_WhenServiceReturnsAccountResponse()
    {
        var svc = new Mock<IAccountsService>();
        var accountId = Guid.NewGuid();
        var accountResponse = new AccountResponse(accountId, "John", "Doe", []);
        svc.Setup(s => s.GetAccountById(accountId))
           .ReturnsAsync(accountResponse);
        var controller = new AccountsController(svc.Object);

        var result = await controller.GetAccountById(accountId);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        Assert.Equal(accountResponse, okResult.Value);
        svc.Verify(svc => svc.GetAccountById(accountId), Times.Once);
    }

    [Fact]
    public async Task UpdateAccount_ReturnsBadRequest_WhenAccountIdIsEmpty()
    {
        var svc = new Mock<IAccountsService>();
        var controller = new AccountsController(svc.Object);
        var updateDto = new UpdateAccountDto(Guid.NewGuid(), "John", "Doe");

        var result = await controller.UpdateAccount(Guid.Empty, updateDto);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        svc.Verify(svc => svc.UpdateAccount(It.IsAny<UpdateAccountDto>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAccount_ReturnsBadRequest_WhenAccountIdDoesNotMatchDtoId()
    {
        var svc = new Mock<IAccountsService>();
        var controller = new AccountsController(svc.Object);
        var updateDto = new UpdateAccountDto(Guid.NewGuid(), "John", "Doe");

        var result = await controller.UpdateAccount(Guid.NewGuid(), updateDto);

        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.BadRequest, badRequestResult.StatusCode);
        svc.Verify(svc => svc.UpdateAccount(It.IsAny<UpdateAccountDto>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAccount_ReturnsOk_WhenServiceReturnsAccountResponse()
    {
        var svc = new Mock<IAccountsService>();
        var accountId = Guid.NewGuid();
        var updateDto = new UpdateAccountDto(accountId, "John", "Doe");
        var accountResponse = new AccountResponse(accountId, "John", "Doe", []);
        svc.Setup(s => s.UpdateAccount(updateDto))
           .ReturnsAsync(accountResponse);
        var controller = new AccountsController(svc.Object);

        var result = await controller.UpdateAccount(accountId, updateDto);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        Assert.Equal(accountResponse, okResult.Value);
        svc.Verify(svc => svc.UpdateAccount(updateDto), Times.Once);
    }

    [Fact]
    public async Task GetAccounts_ReturnsBadRequest_WhenPageNumberIsLessThanOne()
    {
        var svc = new Mock<IAccountsService>();
        var controller = new AccountsController(svc.Object);
        var pagingParams = new PagingParameters()
        {
            PageNumber = 0,
            PageSize = 10
        };

        var result = await controller.GetAccounts(pagingParams);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.BadRequest, badRequest.StatusCode);
        svc.Verify(s => s.GetAccounts(pagingParams), Times.Never);
    }

    [Fact]
    public async Task GetAccounts_ReturnsBadRequest_WhenPageSizeIsLessThanOne()
    {
        var svc = new Mock<IAccountsService>();
        var controller = new AccountsController(svc.Object);
        var pagingParams = new PagingParameters()
        {
            PageNumber = 1,
            PageSize = 0
        };

        var result = await controller.GetAccounts(pagingParams);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.BadRequest, badRequest.StatusCode);
        svc.Verify(s => s.GetAccounts(pagingParams), Times.Never);
    }


    [Fact]
    public async Task GetAccounts_ReturnsBadRequest_WhenPageSizeIsGreaterThanOneHundred()
    {
        var svc = new Mock<IAccountsService>();
        var controller = new AccountsController(svc.Object);
        var pagingParams = new PagingParameters()
        {
            PageNumber = 1,
            PageSize = 101
        };

        var result = await controller.GetAccounts(pagingParams);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.BadRequest, badRequest.StatusCode);
        svc.Verify(s => s.GetAccounts(pagingParams), Times.Never);
    }

    [Fact]
    public async Task GetAccounts_ReturnsOk_WithDefaultParameters()
    {
        var svc = new Mock<IAccountsService>();
        var accountId = Guid.NewGuid();
        var accountResponse = new AccountResponse(accountId, "John", "Doe", new List<Trade>());

        var paginated = new PaginatedResponse<AccountResponse>(
            new List<AccountResponse> { accountResponse },
            PageNumber: 1,
            PageSize: 10,
            TotalItems: 1,
            TotalPages: 1);
        var pagingParams = new PagingParameters()
        {
            PageNumber = 1,
            PageSize = 10
        };

        svc.Setup(s => s.GetAccounts(pagingParams))
           .ReturnsAsync(paginated);

        var controller = new AccountsController(svc.Object);

        var result = await controller.GetAccounts(pagingParams);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        Assert.Equal(paginated, okResult.Value);

        svc.Verify(s => s.GetAccounts(pagingParams), Times.Once);
    }

    [Fact]
    public async Task GetAccounts_ReturnsOk_WithCustomParameters()
    {
        var svc = new Mock<IAccountsService>();
        var accountId = Guid.NewGuid();
        var accountResponse = new AccountResponse(accountId, "John", "Doe", new List<Trade>());

        var paginated = new PaginatedResponse<AccountResponse>(
            new List<AccountResponse> { accountResponse },
            PageNumber: 2,
            PageSize: 1,
            TotalItems: 2,
            TotalPages: 2);
        var pagingParams = new PagingParameters()
        {
            PageNumber = 2,
            PageSize = 1
        };

        svc.Setup(s => s.GetAccounts(pagingParams))
           .ReturnsAsync(paginated);

        var controller = new AccountsController(svc.Object);

        var result = await controller.GetAccounts(pagingParams);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        Assert.Equal(paginated, okResult.Value);

        svc.Verify(s => s.GetAccounts(pagingParams), Times.Once);
    }

    [Fact]
    public async Task SearchAccounts_ReturnsBadRequest_WhenPageNumberLessThanOne()
    {
        var svc = new Mock<IAccountsService>();
        var controller = new AccountsController(svc.Object);

        var searchParams = new SearchPagingParameters { PageNumber = 0, PageSize = 10 };

        var result = await controller.SearchAccounts(searchParams);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.BadRequest, badRequest.StatusCode);
        svc.Verify(s => s.SearchAccounts(It.IsAny<SearchPagingParameters>()), Times.Never);
    }

    [Fact]
    public async Task SearchAccounts_ReturnsBadRequest_WhenPageSizeOutOfRange()
    {
        var svc = new Mock<IAccountsService>();
        var controller = new AccountsController(svc.Object);

        var searchParams = new SearchPagingParameters { PageNumber = 1, PageSize = 0 };

        var result = await controller.SearchAccounts(searchParams);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.BadRequest, badRequest.StatusCode);
        svc.Verify(s => s.SearchAccounts(It.IsAny<SearchPagingParameters>()), Times.Never);
    }

    [Fact]
    public async Task SearchAccounts_ReturnsOk_WhenSearchAccountsIsCalled()
    {
        var svc = new Mock<IAccountsService>();

        var paginated = new PaginatedResponse<AccountResponse>(
            new List<AccountResponse> { 
                new AccountResponse(Guid.NewGuid(), "John", "Doe", new List<Trade>()) },
            PageNumber: 1,
            PageSize: 10,
            TotalItems: 1,
            TotalPages: 1);

        svc.Setup(s => s.SearchAccounts(It.Is<SearchPagingParameters>(
                p => p.PageNumber == 1 && p.PageSize == 10 && p.LastName == "Smith")))
           .ReturnsAsync(paginated);

        var controller = new AccountsController(svc.Object);

        var searchParams = new SearchPagingParameters {
            PageNumber = 1,
            PageSize = 10,
            LastName = "Smith" };

        var result = await controller.SearchAccounts(searchParams);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal((int)HttpStatusCode.OK, okResult.StatusCode);
        Assert.Equal(paginated, okResult.Value);

        svc.Verify(s => s.SearchAccounts(It.Is<SearchPagingParameters>(
            p => p.PageNumber == 1 && p.PageSize == 10 && p.LastName == "Smith")), Times.Once);
    }
}

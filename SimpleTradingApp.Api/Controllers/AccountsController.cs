using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SimpleTradingApp.Application.DTOs;
using SimpleTradingApp.Application.ServiceContracts;

namespace SimpleTradingApp.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountsController : ControllerBase
{
    private readonly IAccountsService _accountsService;

    public AccountsController(IAccountsService accountsService)
    {
        _accountsService = accountsService;
    }

    [HttpPost]
    public async Task<ActionResult<AccountResponse>> AddAccount(AddAccountDto createAccountDto)
    {
        var addedAccount = await _accountsService.AddAccount(createAccountDto);
        if (addedAccount == null)
        {
            return Problem("Unsuccessful in adding account");
        }

        return Ok(addedAccount);
    }

    [HttpDelete("{accountId}")]
    public async Task<ActionResult<bool>> DeleteAccount(Guid accountId)
    {
        if (accountId == Guid.Empty)
        {
            return BadRequest("Account ID is required.");
        }

        var result = await _accountsService.DeleteAccount(accountId);

        if (!result)
        {
            return NotFound(new { Message = $"Account with ID {accountId} not found."});
        }

        return Ok(result);
    }

    [HttpPut("{accountId}")]
    public async Task<ActionResult<AccountResponse>> UpdateAccount(Guid accountId, UpdateAccountDto updateAccountDto)
    {
        if (accountId == Guid.Empty || accountId != updateAccountDto.ID)
        {
            return BadRequest("Account ID is required and must match the ID in the request body.");
        }

        var updatedAccount = await _accountsService.UpdateAccount(updateAccountDto);
        if (updatedAccount == null)
        {
            return NotFound(new { Message = $"Account with ID {accountId} not found."});
        }

        return Ok(updatedAccount);
    }

    [HttpGet("{accountId}")]
    public async Task<ActionResult<AccountResponse>> GetAccountById(Guid accountId)
    {
        if (accountId == Guid.Empty)
        {
            return BadRequest("Account ID is required.");
        }

        var account = await _accountsService.GetAccountById(accountId);
        if (account == null)
        {
            return NotFound(new { Message = $"Account with ID {accountId} not found."});
        }

        return Ok(account);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedResponse<AccountResponse>>> GetAccounts(
        [FromQuery] PagingParameters pagingParams)
    {
        if (pagingParams.PageNumber < PagingParameters.MinPageNumber)
            return BadRequest("Page number must be greater than 0.");

        if (pagingParams.PageSize < PagingParameters.MinPageSize ||
                pagingParams.PageSize > PagingParameters.MaxPageSize)
            return BadRequest("Page size must be between 1 and 100");

        var result = await _accountsService.GetAccounts(pagingParams);

        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<ActionResult<PaginatedResponse<AccountResponse>>> SearchAccounts(
        [FromQuery] SearchPagingParameters searchParams)
    {

        if (searchParams.PageNumber < PagingParameters.MinPageNumber)
            return BadRequest("Page number must be greater than 0.");

        if (searchParams.PageSize < PagingParameters.MinPageSize ||
                searchParams.PageSize > PagingParameters.MaxPageSize)
            return BadRequest("Page size must be between 1 and 100");

        var result = await _accountsService.SearchAccounts(searchParams);

        return Ok(result);
    }
}

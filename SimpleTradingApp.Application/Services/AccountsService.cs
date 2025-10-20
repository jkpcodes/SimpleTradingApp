using SimpleTradingApp.Application.DTOs;
using SimpleTradingApp.Application.IRepositories;
using SimpleTradingApp.Application.Mappers;
using SimpleTradingApp.Application.ServiceContracts;

namespace SimpleTradingApp.Application.Services;

public class AccountsService : IAccountsService
{
    private readonly IAccountsRepository _accountsRepository;

    public AccountsService(IAccountsRepository accountsRepository)
    {
        _accountsRepository = accountsRepository;
    }

    public async Task<AccountResponse?> AddAccount(AddAccountDto createAccountDto)
    {
        var account = createAccountDto.ToAccount();
        var addedAccount = await _accountsRepository.AddAccount(account);

        return addedAccount?.ToAccountResponse();
    }

    public async Task<bool> DeleteAccount(Guid accountId)
    {
        var account = await _accountsRepository.GetAccountById(accountId);

        if (account == null)
        {
            return false;
        }

        return await _accountsRepository.DeleteAccount(accountId);
    }

    public async Task<AccountResponse?> GetAccountById(Guid accountId)
    {
        var account = await _accountsRepository.GetAccountById(accountId, true);

        if (account == null)
        {
            return null;
        }

        return account.ToAccountResponse();
    }

    public async Task<AccountResponse?> UpdateAccount(UpdateAccountDto updateAccountDto)
    {
        var account = await _accountsRepository.GetAccountById(updateAccountDto.ID);

        if (account == null)
        {
            return null;
        }

        var updatedAccount = await _accountsRepository.UpdateAccount(updateAccountDto.ToAccount());

        return updatedAccount?.ToAccountResponse();
    }

    public async Task<PaginatedResponse<AccountResponse>> GetAccounts(PagingParameters pagingParams)
    {
        if (pagingParams.PageNumber < PagingParameters.MinPageNumber)
            pagingParams.PageNumber = 1;

        if (pagingParams.PageSize < PagingParameters.MinPageSize)
            pagingParams.PageSize = 10;

        var (items, total) = await _accountsRepository.GetAccounts(pagingParams);

        var mappedItems = items.Select(a => a.ToAccountResponse()).ToList();

        var totalPages = (int)Math.Ceiling((double)total / pagingParams.PageSize);

        return new PaginatedResponse<AccountResponse>(
            mappedItems,
            pagingParams.PageNumber,
            pagingParams.PageSize,
            total,
            totalPages
        );
    }
}

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
        return await _accountsRepository.DeleteAccount(accountId);
    }
}

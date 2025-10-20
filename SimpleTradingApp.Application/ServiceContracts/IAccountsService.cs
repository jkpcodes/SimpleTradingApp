using SimpleTradingApp.Application.DTOs;

namespace SimpleTradingApp.Application.ServiceContracts;

public interface IAccountsService
{
    /// <summary>
    /// Add new account
    /// </summary>
    /// <param name="createAccountDto"></param>
    /// <returns>Returns newly added order; otherwise null</returns>
    Task<AccountResponse?> AddAccount(AddAccountDto createAccountDto);

    /// <summary>
    /// Delete account and trades in that account if there are any
    /// </summary>
    /// <param name="accountId"></param>
    /// <returns></returns>
    Task<bool> DeleteAccount(Guid accountId);
}

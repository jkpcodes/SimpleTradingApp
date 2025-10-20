using SimpleTradingApp.Application.DTOs;

namespace SimpleTradingApp.Application.ServiceContracts;

public interface IAccountsService
{
    /// <summary>
    /// Add new account
    /// </summary>
    /// <param name="createAccountDto"></param>
    /// <returns>Returns newly added account; otherwise null</returns>
    Task<AccountResponse?> AddAccount(AddAccountDto createAccountDto);

    /// <summary>
    /// Delete account and trades in that account if there are any
    /// </summary>
    /// <param name="accountId"></param>
    /// <returns></returns>
    Task<bool> DeleteAccount(Guid accountId);

    /// <summary>
    /// Update account information
    /// </summary>
    /// <param name="updateAccountDto"></param>
    /// <returns>Updated account data; otherwise null</returns>
    Task<AccountResponse?> UpdateAccount(UpdateAccountDto updateAccountDto);

    /// <summary>
    /// Get account data by ID
    /// </summary>
    /// <param name="accountId"></param>
    /// <returns>Account data matching the account ID; otherwise null</returns>
    Task<AccountResponse?> GetAccountById(Guid accountId);
}

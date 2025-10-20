using SimpleTradingApp.Domain.Entities;

namespace SimpleTradingApp.Application.IRepositories;
public interface IAccountsRepository
{
    /// <summary>
    /// Creates a new account
    /// </summary>
    /// <param name="account"></param>
    /// <returns>Returns the newly created account or null if unsuccessful</returns>
    Task<Account?> AddAccount(Account account);

    /// <summary>
    /// Get account data by ID
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="includeTrades"></param>
    /// <returns>Returns account data matched by Id, otherwise null</returns>
    Task<Account?> GetAccountById(Guid accountId, bool includeTrades = false);

    /// <summary>
    /// Delete account data (also deletes trades under that account if there are any)
    /// </summary>
    /// <param name="accountId"></param>
    /// <returns>Returns true if account is successfully deleted, otherwise false</returns>
    Task<bool> DeleteAccount(Guid accountId);

    /// <summary>
    /// Update account data
    /// </summary>
    /// <param name="account"></param>
    /// <returns>Returns the updated account data, otherwise null</returns>
    Task<Account?> UpdateAccount(Account account);
}

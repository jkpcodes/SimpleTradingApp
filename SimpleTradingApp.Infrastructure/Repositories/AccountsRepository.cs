using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SimpleTradingApp.Application.IRepositories;
using SimpleTradingApp.Domain.Entities;

namespace SimpleTradingApp.Infrastructure.Repositories;
public class AccountsRepository : IAccountsRepository
{
    private readonly AppDbContext _context;

    public AccountsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Account?> AddAccount(Account account)
    {
        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();
        return account;
    }

    public async Task<bool> DeleteAccount(Guid accountId)
    {
        var account = await _context.Accounts
            .Include(a => a.Trades)
            .FirstOrDefaultAsync(a => a.ID == accountId);

        if (account == null)
            return false;

        var result = false;
        IDbContextTransaction? transaction = null;
        var providerName = _context.Database.ProviderName ?? string.Empty;
        // InMemory provider does not support transactions
        var supportsTransactions = !providerName.Equals("Microsoft.EntityFrameworkCore.InMemory",
            StringComparison.OrdinalIgnoreCase);

        if (supportsTransactions)
        {
            transaction = await _context.Database.BeginTransactionAsync();
        }
        try
        {
            if (account.Trades.Count > 0)
            {
                _context.Trades.RemoveRange(account.Trades);
            }

            _context.Accounts.Remove(account);

            await _context.SaveChangesAsync();

            result = true;
        }
        catch
        {
            result = false;
        }
        finally
        {
            if (transaction != null)
            {
                if (result)
                    await transaction.CommitAsync();
                else
                    await transaction.RollbackAsync();
            }
        }

        return result;
    }
}

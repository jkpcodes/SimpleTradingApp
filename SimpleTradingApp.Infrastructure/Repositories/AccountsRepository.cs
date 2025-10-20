using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SimpleTradingApp.Application.DTOs;
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

    public async Task<Account?> GetAccountById(Guid accountId, bool includeTrades = false)
    {
        var query = _context.Accounts.AsQueryable();

        if (includeTrades)
        {
            query = query.Include(a => a.Trades);
        }

        return await query
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.ID == accountId);
    }

    public async Task<Account?> UpdateAccount(Account account)
    {
        _context.Accounts.Update(account);
        var result = await _context.SaveChangesAsync();

        return result == 1 ? account : null;
    }

    public async Task<(IEnumerable<Account> Accounts, int TotalCount)> GetAccounts(
        PagingParameters pagingParams)
    {
        if (pagingParams.PageNumber < PagingParameters.MinPageNumber)
            pagingParams.PageNumber = 1;

        if (pagingParams.PageSize < PagingParameters.MinPageSize)
            pagingParams.PageSize = 10;

        // Order by LastName, then FirstName by default
        var query = _context.Accounts.AsNoTracking()
            .OrderBy(a => a.LastName)
            .ThenBy(a => a.FirstName);

        var total = await query.CountAsync();

        var items = await query
            .Skip((pagingParams.PageNumber - 1) * pagingParams.PageSize)
            .Take(pagingParams.PageSize)
            .ToListAsync();

        return (items, total);
    }

    public async Task<(IEnumerable<Account> Accounts, int TotalCount)> SearchAccounts(
        SearchPagingParameters searchParams)
    {
        if (searchParams.PageNumber < PagingParameters.MinPageNumber)
            searchParams.PageNumber = 1;

        if (searchParams.PageSize < PagingParameters.MinPageSize)
            searchParams.PageSize = 10;

        var query = _context.Accounts.AsNoTracking().AsQueryable();

        // Apply ID filter if provided and valid
        if (!string.IsNullOrWhiteSpace(searchParams.ID))
        {
            query = query.Where(a => a.ID.ToString().Contains(searchParams.ID));
        }

        // Apply LastName filter if provided (case-insensitive, partial match)
        if (!string.IsNullOrWhiteSpace(searchParams.LastName))
        {
            var lastNameFilter = searchParams.LastName.Trim().ToLower();
            query = query.Where(a => a.LastName.ToLower().Contains(lastNameFilter));
        }

        // Order by LastName, then FirstName
        query = query.AsNoTracking()
            .OrderBy(a => a.LastName)
            .ThenBy(a => a.FirstName);

        var total = await query.CountAsync();

        var items = await query
            .Skip((searchParams.PageNumber - 1) * searchParams.PageSize)
            .Take(searchParams.PageSize)
            .ToListAsync();

        return (items, total);
    }
}

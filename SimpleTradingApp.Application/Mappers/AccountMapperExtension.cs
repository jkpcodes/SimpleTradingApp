using SimpleTradingApp.Application.DTOs;
using SimpleTradingApp.Domain.Entities;

namespace SimpleTradingApp.Application.Mappers;

public static class AccountMapperExtension
{
    public static AccountResponse ToAccountResponse(this Account account)
    {
        return new AccountResponse(
            account.ID,
            account.FirstName,
            account.LastName,
            account.Trades.ToList()
        );
    }
}

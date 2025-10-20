using SimpleTradingApp.Application.DTOs;
using SimpleTradingApp.Domain.Entities;

namespace SimpleTradingApp.Application.Mappers;

public static class AddAccountDtoMapperExtension
{
    public static Account ToAccount(this AddAccountDto dto)
    {
        return new Account
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };
    }
}

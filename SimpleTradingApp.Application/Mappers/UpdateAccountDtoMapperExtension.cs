using SimpleTradingApp.Domain.Entities;

namespace SimpleTradingApp.Application.Mappers;

public static class UpdateAccountDtoMapperExtension
{
    public static Account ToAccount(this DTOs.UpdateAccountDto dto)
    {
        return new Account
        {
            ID = dto.ID,
            FirstName = dto.FirstName,
            LastName = dto.LastName
        };
    }
}

namespace SimpleTradingApp.Application.DTOs;

public record UpdateAccountDto(
    Guid ID,
    string FirstName,
    string LastName
);

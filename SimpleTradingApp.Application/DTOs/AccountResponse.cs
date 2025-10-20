using SimpleTradingApp.Domain.Entities;

namespace SimpleTradingApp.Application.DTOs;

public record AccountResponse(
    Guid ID,
    string FirstName,
    string LastName,
    List<Trade> Trades
);

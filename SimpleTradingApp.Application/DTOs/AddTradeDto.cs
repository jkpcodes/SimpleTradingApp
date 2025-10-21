using SimpleTradingApp.Domain.Entities;

namespace SimpleTradingApp.Application.DTOs;

public record AddTradeDto(
    Guid AccountId,
    string SecurityCode,
    decimal Amount,
    TradeType Type
);

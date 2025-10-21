using SimpleTradingApp.Domain.Entities;

namespace SimpleTradingApp.Application.DTOs;

public record UpdateTradeStatusDto(
    Guid ID,
    TradeStatus Status
);

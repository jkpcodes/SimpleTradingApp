using SimpleTradingApp.Application.DTOs;
using SimpleTradingApp.Domain.Entities;

namespace SimpleTradingApp.Application.Mappers;

public static class TradeMapperExtension
{
    public static TradeResponse ToTradeResponse(this Trade trade)
    {
        return new TradeResponse(
            ID: trade.ID,
            AccountId: trade.AccountId,
            SecurityCode: trade.SecurityCode,
            Timestamp: trade.Timestamp,
            Amount: trade.Amount,
            Type: trade.Type,
            Status: trade.Status
        );
    }
}

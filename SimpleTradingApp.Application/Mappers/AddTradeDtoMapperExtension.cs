using SimpleTradingApp.Application.DTOs;
using SimpleTradingApp.Domain.Entities;

namespace SimpleTradingApp.Application.Mappers;

public static class AddTradeDtoMapperExtension
{
    public static Trade ToTrade(this AddTradeDto dto)
    {
        return new Trade
        {
            AccountId = dto.AccountId,
            SecurityCode = dto.SecurityCode,
            Amount = dto.Amount,
            Type = dto.Type,
            Status = TradeStatus.Placed
        };
    }
}

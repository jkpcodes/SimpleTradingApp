using SimpleTradingApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTradingApp.Application.DTOs;

public record TradeResponse(
    Guid ID,
    Guid AccountId,
    string SecurityCode,
    DateTime Timestamp,
    decimal Amount,
    TradeType Type,
    TradeStatus Status
);
namespace SimpleTradingApp.Domain.Entities;

public enum TradeType
{
    Buy,
    Sell
}

public enum TradeStatus
{
    Placed,
    Executed,
    Expired
}

public class Trade
{
    public Guid ID { get; set; }
    public Guid AccountId { get; set; }
    public string SecurityCode { get; set; }
    public DateTime Timestamp { get; set; }
    public decimal Amount { get; set; }
    public TradeType Type { get; set; }
    public TradeStatus Status { get; set; }
}

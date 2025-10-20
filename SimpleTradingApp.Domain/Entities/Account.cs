using System.Text.Json.Serialization;

namespace SimpleTradingApp.Domain.Entities;

public class Account
{
    public Guid ID { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    [JsonIgnore]
    public ICollection<Trade> Trades { get; set; } = new List<Trade>();
}

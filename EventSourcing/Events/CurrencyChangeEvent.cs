using System.Text.Json.Serialization;
using EventSourcing.Models;

namespace EventSourcing.Events;

public record CurrencyChangeEvent : Event
{
  [JsonPropertyName("newBalance")] 
  public required decimal NewBalance { get; init; }

  [JsonPropertyName("newCurrency")]
  public required CurrencyType NewCurrency { get; init; }
}

using System.Text.Json.Serialization;

namespace Hiredaily.Modules.Feed.Application.Common;

public sealed record Money
{
    [JsonPropertyName("amount")]
    public decimal Amount { get; init; }
    [JsonPropertyName("currency")]
    public string Currency { get; init; } = default!;
    [JsonPropertyName("isZero")]
    public bool IsZero { get; init; }
}
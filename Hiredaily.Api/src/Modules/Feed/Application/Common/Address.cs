using System.Text.Json.Serialization;

namespace Hiredaily.Modules.Feed.Application.Write;

public sealed record Address
{
    [JsonPropertyName("addressLine1")]
    public string AddressLine1 { get; init; } = default!;
    [JsonPropertyName("addressLine2")]
    public string AddressLine2 { get; init; } = default!;
    [JsonPropertyName("city")]
    public string City { get; init; } = default!;
    [JsonPropertyName("state")]
    public string State { get; init; } = default!;
    [JsonPropertyName("country")]
    public string Country { get; init; } = default!;
    [JsonPropertyName("postalCode")]
    public string PostalCode { get; init; } = default!;
}

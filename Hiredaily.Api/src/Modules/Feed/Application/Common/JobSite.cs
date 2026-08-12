using System.Text.Json.Serialization;
using Hiredaily.Modules.Feed.Application.Write;

namespace Hiredaily.Modules.Feed.Application.Common;

public sealed record JobSite
{
    [JsonPropertyName("location")]
    public Location Location { get; init; } = default!;
    [JsonPropertyName("address")]
    public Address Address { get; init; } = default!;
}

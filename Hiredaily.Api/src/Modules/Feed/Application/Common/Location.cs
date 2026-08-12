using System.Text.Json.Serialization;

namespace Hiredaily.Modules.Feed.Application.Common;

public sealed record Location
{
    [JsonPropertyName("lat")]
    public string Lat { get; init; } = default!;
    [JsonPropertyName("long")]
    public string Long { get; init; } = default!;
}

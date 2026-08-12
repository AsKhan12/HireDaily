using System.Text.Json.Serialization;

namespace Hiredaily.Modules.Feed.Application.Common;

public sealed record JobId
{
    [JsonPropertyName("value")]
    public Guid Value { get; init; }
}

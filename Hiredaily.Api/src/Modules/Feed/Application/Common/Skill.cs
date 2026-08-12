using System.Text.Json.Serialization;

namespace Hiredaily.Modules.Feed.Application.Common;

public sealed record Skill
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = default!;
    [JsonPropertyName("field")]
    public string Field { get; init; } = default!;
    [JsonPropertyName("description")]
    public string Description { get; init; } = default!;
    [JsonPropertyName("skillLevel")]
    public int SkillLevel { get; init; }
}

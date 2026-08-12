using Hiredaily.BuildingBlock.Domain.Enums;

namespace Hiredaily.BuildingBlock.Domain.ValueObjects;

public record Skill(string Name, string Field, string Description, SkillLevel SkillLevel);
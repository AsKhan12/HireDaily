using Hiredaily.BuildingBlock.Domain.Enums;

namespace Hiredaily.Modules.Jobs.Application.Shared;

public class SkillDto
{
    public string Name { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public SkillLevel SkillLevel { get; set; }
}

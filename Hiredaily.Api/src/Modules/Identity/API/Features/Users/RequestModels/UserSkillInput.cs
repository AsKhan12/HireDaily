using Hiredaily.BuildingBlock.Domain.Enums;

namespace Hiredaily.Modules.Identity.API.Features.Users.RequestModels;

public class UserSkillInput
{
    public string Name { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public SkillLevel SkillLevel { get; set; }
}

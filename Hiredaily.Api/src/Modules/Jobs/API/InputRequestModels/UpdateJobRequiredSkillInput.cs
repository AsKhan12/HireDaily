using Hiredaily.Modules.Jobs.Application.Shared;

namespace Hiredaily.Modules.Jobs.API.InputRequestModels;

public class UpdateJobRequiredSkillInput
{
    public string RequestedBy { get; set; } = string.Empty;
    public IReadOnlyList<SkillDto> RequiredSkills { get; set; } = [];
}
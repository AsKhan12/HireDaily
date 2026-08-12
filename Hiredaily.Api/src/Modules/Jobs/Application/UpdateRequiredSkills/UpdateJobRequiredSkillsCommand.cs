using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Application.Mediator.Requests;
using Hiredaily.Modules.Jobs.Application.Shared;

namespace Hiredaily.Modules.Jobs.Application.UpdateRequiredSkills;

public class UpdateJobRequiredSkillsCommand : ICommand
{
    public Guid RequestId { get; set; }
    public DateTime RequestedAt { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public JobId JobId { get; set; } = default!;
    public IReadOnlyList<SkillDto> RequiredSkills { get; set; } = [];
}

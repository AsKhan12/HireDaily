using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Domain.ValueObjects;
using Hiredaily.Modules.Jobs.Application.Shared;
using Hiredaily.Modules.Jobs.Domain.ValueObjects;

namespace Hiredaily.Modules.Jobs.Application.GetOrganizationJob;

public class GetOrganizationJobResponse
{
    public List<JobsDto>? Jobs {get; set;}
}
public class JobsDto
{
    public JobId JobId { get; set; } = default!;
    public OrganizationId OrganizationId { get; set; } = default!;
    public Money HourlyRate { get; set; } = default!;
    public JobSite JobSite { get; set; } = default!;
    public IReadOnlyList<SkillDto> RequiredSkills { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdateAt { get; set; }
}
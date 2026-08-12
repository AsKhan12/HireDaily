using Hiredaily.BuildingBlock.Application.Events;
using Hiredaily.Modules.Feed.Application.Common;


namespace Hiredaily.Modules.Feed.Application.Write;

public record JobCreatedMessage(
    JobId JobId,
    string Title,
    DateTime JobCreatedAt,
    DateTime? JobLastUpdatedAt,
    OrganizationId OrganizationId,
    IReadOnlyList<Skill> RequiredSkills,
    JobSite JobSite,
    Money HourlyRate);
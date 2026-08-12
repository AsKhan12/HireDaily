using Hiredaily.BuildingBlock.Application.Events;
using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Domain.ValueObjects;
using Hiredaily.Modules.Jobs.Domain.ValueObjects;

namespace Hiredaily.Modules.Jobs.Application.CreateJob;

public record JobCreatedMessagePayload(
    JobId JobId,
    string Title,
    DateTime JobCreatedAt,
    DateTime? JobLastUpdatedAt,
    OrganizationId OrganizationId,
    IReadOnlyList<Skill> RequiredSkills,
    JobSite JobSite,
    Money HourlyRate) : IMessagePayload;
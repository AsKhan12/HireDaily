using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Domain.Events;
using Hiredaily.BuildingBlock.Domain.ValueObjects;
using Hiredaily.Modules.Jobs.Domain.ValueObjects;

namespace Hiredaily.Modules.Jobs.Domain.Events;

public record JobCreatedEvent : IDomainEvent
{
    public JobId JobId { get; }
    public string Title { get; }
    public DateTime JobCreatedAt { get; }
    public DateTime? JobLastUpdatedAt { get; }
    public OrganizationId OrganizationId { get; }
    public IReadOnlyList<Skill> RequiredSkills { get; }
    public JobSite JobSite { get; }
    public Money HourlyRate { get; }
    public JobCreatedEvent(
        JobId aggregateId,
        DateTime jobCreatedAt,
        DateTime? jobLastUpdatedAt,
        OrganizationId organizationId,
        IReadOnlyList<Skill> requiredSkills,
        JobSite jobSite,
        Money hourlyRate,
        string title)
    {
        JobId = aggregateId;
        JobCreatedAt = jobCreatedAt;
        JobLastUpdatedAt = jobLastUpdatedAt;
        OrganizationId = organizationId;
        RequiredSkills = requiredSkills;
        JobSite = jobSite;
        HourlyRate = hourlyRate;
        Title = title;
    }
}

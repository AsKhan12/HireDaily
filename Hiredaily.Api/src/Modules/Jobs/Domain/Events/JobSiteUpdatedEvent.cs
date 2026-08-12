using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Domain.Events;
using Hiredaily.Modules.Jobs.Domain.ValueObjects;

namespace Hiredaily.Modules.Jobs.Domain.Events;

public record JobSiteUpdatedEvent : IDomainEvent
{
    public JobId AggregateId { get; }
    public JobSite JobSite { get; }

    public JobSiteUpdatedEvent(JobId aggregateId, JobSite jobSite)
    {
        AggregateId = aggregateId;
        JobSite = jobSite;
    }
}

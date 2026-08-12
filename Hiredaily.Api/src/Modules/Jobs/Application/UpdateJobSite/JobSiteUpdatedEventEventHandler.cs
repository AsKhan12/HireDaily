using Hiredaily.BuildingBlock.Application.Events;
using Hiredaily.Modules.Jobs.Domain.Events;
using Hiredaily.Modules.Jobs.Application.Outbox;

namespace Hiredaily.Modules.Jobs.Application.UpdateJobSite;

public class JobSiteUpdatedEventHandler(IOutboxRepository repository) : IDomainEventHandler<JobSiteUpdatedEvent>
{
    public Task Handle(JobSiteUpdatedEvent evt)
    {
        var integrationEvent = new JobSiteUpdatedMessage
        {
            EventData = new JobSiteUpdatedMessagePayload(evt.AggregateId, evt.JobSite),
            CreatedAt = DateTime.UtcNow,
            EventId = Guid.NewGuid()
        };
        repository.AddMessage<JobSiteUpdatedMessage, JobSiteUpdatedMessagePayload>(integrationEvent);
        return Task.CompletedTask;
    }
}

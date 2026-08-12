using Hiredaily.BuildingBlock.Application.Events;
using Hiredaily.Modules.Jobs.Application.Outbox;
using Hiredaily.Modules.Jobs.Domain.Events;

namespace Hiredaily.Modules.Jobs.Application.CreateJob;

public class JobCreatedEventHandler(IOutboxRepository repository) : IDomainEventHandler<JobCreatedEvent>
{
    public Task Handle(JobCreatedEvent evt)
    {
        var integrationEvent = new JobCreatedMessage
        {
            EventData = new JobCreatedMessagePayload
            (
                evt.JobId,
                evt.Title,
                evt.JobCreatedAt,
                evt.JobLastUpdatedAt,
                evt.OrganizationId,
                evt.RequiredSkills,
                evt.JobSite,
                evt.HourlyRate

            ),
            CreatedAt = DateTime.UtcNow,
            EventId = Guid.NewGuid()
        };
        repository.AddMessage<JobCreatedMessage, JobCreatedMessagePayload>(integrationEvent);
        return Task.CompletedTask;
    }
}
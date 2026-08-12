using Hiredaily.Modules.Jobs.Domain.Events;
using Hiredaily.Modules.Jobs.Application.Outbox;
using Hiredaily.BuildingBlock.Application.Events;

namespace Hiredaily.Modules.Jobs.Application.UpdateJobTitle;

public class JobTitleUpdatedEventHandler(IOutboxRepository repository) : IDomainEventHandler<JobTitleUpdatedEvent>
{
    public Task Handle(JobTitleUpdatedEvent evt)
    {
        var integrationEvent = new JobTitleUpdatedMessage
        {
            EventData = new JobTitleMessagePayload(evt.Title, evt.AggregateId),
            CreatedAt = DateTime.UtcNow,
            EventId = Guid.NewGuid()
        };
        repository.AddMessage<JobTitleUpdatedMessage, JobTitleMessagePayload>(integrationEvent);
        return Task.CompletedTask;
    }
}
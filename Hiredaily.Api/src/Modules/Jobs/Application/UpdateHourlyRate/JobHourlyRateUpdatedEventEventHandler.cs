using Hiredaily.BuildingBlock.Application.Events;
using Hiredaily.Modules.Jobs.Application.Outbox;
using Hiredaily.Modules.Jobs.Domain.Events;

namespace Hiredaily.Modules.Jobs.Application.UpdateHourlyRate;

public class JobHourlyRateUpdatedEventEventHandler(IOutboxRepository repository) : IDomainEventHandler<JobHourlyRateUpdatedEvent>
{
    public Task Handle(JobHourlyRateUpdatedEvent evt)
    {
        var integrationEvent = new JobHourlyRateUpdatedMessage
        {
            EventData = new JobHourlyRateUpdatedPayload(evt.AggregateId, evt.HourlyRate),
            CreatedAt = DateTime.UtcNow,
            EventId = Guid.NewGuid()
        };
        repository.AddMessage<JobHourlyRateUpdatedMessage, JobHourlyRateUpdatedPayload>(integrationEvent);
        return Task.CompletedTask;
    }
}

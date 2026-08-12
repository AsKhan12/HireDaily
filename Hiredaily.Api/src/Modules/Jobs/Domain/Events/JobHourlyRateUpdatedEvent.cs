using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Domain.Events;
using Hiredaily.BuildingBlock.Domain.ValueObjects;

namespace Hiredaily.Modules.Jobs.Domain.Events;

public record JobHourlyRateUpdatedEvent : IDomainEvent
{
    public JobId AggregateId { get; }
    public Money HourlyRate { get; }

    public JobHourlyRateUpdatedEvent(JobId aggregateId, Money hourlyRate)
    {
        AggregateId = aggregateId;
        HourlyRate = hourlyRate;
    }
}
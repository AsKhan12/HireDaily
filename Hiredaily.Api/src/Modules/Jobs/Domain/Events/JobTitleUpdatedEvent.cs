using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Domain.Events;

namespace Hiredaily.Modules.Jobs.Domain.Events;

public record JobTitleUpdatedEvent(JobId AggregateId, string Title) : IDomainEvent;
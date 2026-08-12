using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Domain.Events;

namespace Hiredaily.Modules.Identity.Domain.User.Events;

public sealed record UserAddressUpdatedEvent : IDomainEvent
{
    public UserId AggregateId {get;}

    public UserAddressUpdatedEvent(UserId aggregateId)
    {
        AggregateId = aggregateId;
    }
}
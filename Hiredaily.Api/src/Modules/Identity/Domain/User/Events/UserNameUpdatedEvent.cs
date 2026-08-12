using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Domain.Events;

namespace Hiredaily.Modules.Identity.Domain.User.Events;

public sealed record UserNameUpdatedEvent : IDomainEvent
{
    public UserId AggregateId {get;}
    public string UserName {get; }

    public UserNameUpdatedEvent(UserId aggregateId, string name)
    {
        AggregateId = aggregateId;
        UserName = name;
    }
}
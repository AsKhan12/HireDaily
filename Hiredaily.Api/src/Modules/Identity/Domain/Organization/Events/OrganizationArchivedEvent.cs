using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Domain.Events;

namespace Hiredaily.Modules.Identity.Domain.Organization.Events;

public sealed record OrganizationArchivedEvent : IDomainEvent
{
    public OrganizationId AggregateId {get;}

    public OrganizationArchivedEvent(OrganizationId aggregateId)
    {
        AggregateId = aggregateId;
    }
}
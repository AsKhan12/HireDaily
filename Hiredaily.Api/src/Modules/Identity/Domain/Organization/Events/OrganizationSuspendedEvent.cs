using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Domain.Events;

namespace Hiredaily.Modules.Identity.Domain.Organization.Events;

public sealed record OrganizationSuspendedEvent : IDomainEvent
{
    public OrganizationId OrganizationId { get; }
    public OrganizationSuspendedEvent(OrganizationId aggregateId)
    {
        OrganizationId = aggregateId;
    }
}
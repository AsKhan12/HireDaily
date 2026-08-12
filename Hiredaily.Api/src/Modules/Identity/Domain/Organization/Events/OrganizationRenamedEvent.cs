using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Domain.Events;

namespace Hiredaily.Modules.Identity.Domain.Organization.Events;

public sealed record OrganizationRenamedEvent : IDomainEvent
{
    public OrganizationId OrganizationId { get; }
    public string UpdatedName { get; }

    public OrganizationRenamedEvent(OrganizationId aggregateId, string updatedName)
    {
        OrganizationId = aggregateId;
        UpdatedName = updatedName;
    }
}
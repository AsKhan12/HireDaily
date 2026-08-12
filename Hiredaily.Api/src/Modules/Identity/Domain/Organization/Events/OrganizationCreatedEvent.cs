using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Domain.Events;
using Hiredaily.Modules.Identity.Domain.Organization.ValueObjects;

namespace Hiredaily.Modules.Identity.Domain.Organization.Events;

public sealed record OrganizationCreatedEvent : IDomainEvent
{

    public OrganizationId AggregateId { get; }
    public string Name { get; private set; } = string.Empty;

    public string? Description { get; private set; }

    public OrganizationAddress? Address { get; private set; }

    public OrganizationStatusEnum Status { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public OrganizationCreatedEvent(OrganizationId aggregateId, DateTime? updatedAt, DateTime createdAt, OrganizationStatusEnum status, OrganizationAddress? address, string? description, string name)
    {
        AggregateId = aggregateId;
        UpdatedAt = updatedAt;
        Status = status;
        Address = address;
        Description = description;
        Name = name;
    }
}

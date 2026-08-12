using Hiredaily.BuildingBlock.Domain.Events;
using Hiredaily.Modules.Identity.Domain.Organization.ValueObjects;

namespace Hiredaily.Modules.Identity.Domain.Organization.Events;

public sealed record OrganizationAddressUpdatedEvent : IDomainEvent
{
    public OrganizationAddress UpdatedAddress {get;}

    public OrganizationAddressUpdatedEvent(OrganizationAddress updatedAddress)
    {
        UpdatedAddress = updatedAddress;
    }
}
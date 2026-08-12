using Hiredaily.BuildingBlock.Domain.Events;

namespace Hiredaily.Modules.Identity.Domain.Organization.Events;

public sealed record OrganizationActivatedEvent : IDomainEvent
{
    public Organization Organization {get;}

    public OrganizationActivatedEvent(Organization organization)
    {
        Organization = organization;
    }
}
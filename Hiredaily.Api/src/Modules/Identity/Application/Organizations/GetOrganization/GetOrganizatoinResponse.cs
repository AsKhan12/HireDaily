using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.Modules.Identity.Domain.Organization;
using Hiredaily.Modules.Identity.Domain.Organization.ValueObjects;

namespace Hiredaily.Modules.Identity.Application.Organizations.GetOrganization;

public class GetOrganizationResponse
{
    public OrganizationId? OrganizationId { get; init; }
    public string? OrganizationName { get; init;}
    public string? OrganizationDescription { get; init;}
    public string Username { get; init; } = default!;
    public string? Description { get; init; }

    public OrganizationAddress Address { get; init; } = OrganizationAddress.Empty();

    public OrganizationStatusEnum Status { get; init; }
    
}

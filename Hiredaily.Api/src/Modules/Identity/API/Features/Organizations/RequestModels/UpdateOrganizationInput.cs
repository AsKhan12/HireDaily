namespace Hiredaily.Modules.Identity.API.Features.Organizations.RequestModels;
public class UpdateOrganizationInput
{
    public Guid RequestId { get; init; }
    public DateTime RequestedAt { get; init; }
    public string? RequestedBy { get; init; }
    public required OrganizationIdDto OrganizationId { get; init; }
    public string? UpdatedName { get; init; }
    public string? UpdatedDescription { get; init; }
    public UpdateOrganizationAddressDto? UpdatedAddress { get; init; }
}

public class OrganizationIdDto
{
    public Guid Value { get; init; }
}

public class UpdateOrganizationAddressDto
{
    public bool IsInitialized { get; init; }
    public GeoLocationDto? Location { get; init; }
    public PostalAddressDto? PostalAddress { get; init; }
    public OrganizationContactDetailsDto? ContactDetails { get; init; }
}

public class GeoLocationDto
{
    public string Lat { get; init; } = default!;
    public string Long { get; init; } = default!;
}

public class PostalAddressDto
{
    public string AddressLine1 { get; init; } = default!;
    public string? AddressLine2 { get; init; }
    public string City { get; init; } = default!;
    public string State { get; init; } = default!;
    public string Country { get; init; } = default!;
    public string PostalCode { get; init; } = default!;
}

public class OrganizationContactDetailsDto
{
    public string Email { get; init; } = default!;
    public string Phone { get; init; } = default!;
    public string WebsiteUrl { get; init; } = default!;
}

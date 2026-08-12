using Hiredaily.BuildingBlock.Domain.ValueObjects;

namespace Hiredaily.Modules.Identity.Domain.Organization.ValueObjects;

public record OrganizationAddress
{
    public bool IsInitialized { get; private set; } = true;

    public GeoLocation Location { get; private set; } = GeoLocation.Empty();

    public PostalAddress PostalAddress { get; private set; } = PostalAddress.Empty();

    public OrganizationContactDetails ContactDetails { get; private set; } = OrganizationContactDetails.Empty();

    private OrganizationAddress() { }

    public static OrganizationAddress Empty() => new();

    public OrganizationAddress(GeoLocation location, PostalAddress postalAddress, OrganizationContactDetails contactDetails)
    {
        Location = location;
        PostalAddress = postalAddress;
        ContactDetails = contactDetails;
    }
}

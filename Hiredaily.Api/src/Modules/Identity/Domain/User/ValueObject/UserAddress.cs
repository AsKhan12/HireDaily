using Hiredaily.BuildingBlock.Domain.ValueObjects;

namespace Hiredaily.Modules.Identity.Domain.User.ValueObject;

public record UserAddress
{
	public bool IsInitialized { get; private set; } = true;

	public GeoLocation Locatoin { get; private set; } = GeoLocation.Empty();

	public PostalAddress PostalAddress { get; private set; } = PostalAddress.Empty();

	public UserContactDetails ContactDetails { get; private set; } = UserContactDetails.Empty();

	private UserAddress() { }

	public static UserAddress Empty() => new();

	public UserAddress(GeoLocation locatoin, PostalAddress postalAddress, UserContactDetails contactDetails)
	{
		Locatoin = locatoin;
		PostalAddress = postalAddress;
		ContactDetails = contactDetails;
	}
}

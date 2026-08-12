
namespace Hiredaily.Modules.Identity.Domain.Organization.ValueObjects;

public record OrganizationContactDetails
{
	public string Email { get; private set; } = default!;

	public string Phone { get; private set; } = default!;

	public string WebsiteUrl { get; private set; } = default!;

	private OrganizationContactDetails() { }

	public static OrganizationContactDetails Empty() => new();

	public OrganizationContactDetails(string email, string phone, string websiteUrl)
	{
		Email = email;
		Phone = phone;
		WebsiteUrl = websiteUrl;
	}
}

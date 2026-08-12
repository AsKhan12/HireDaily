namespace Hiredaily.Modules.Identity.Domain.User.ValueObject;

public record UserContactDetails
{
    public string Phone { get; private set; } = default!;

    public string Email { get; private set; } = default!;

    private UserContactDetails() { }

    public static UserContactDetails Empty() => new();

    public UserContactDetails(string phone, string email)
    {
        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone is required.", nameof(phone));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        if (!email.Contains('@', StringComparison.Ordinal))
            throw new ArgumentException("Email must be valid.", nameof(email));

        Phone = phone;
        Email = email;
    }
}

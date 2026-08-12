namespace Hiredaily.BuildingBlock.Domain.ValueObjects;

public record PostalAddress
{
    public string AddressLine1 { get; private set; } = default!;

    public string? AddressLine2 { get; private set; }

    public string City { get; private set; } = default!;

    public string State { get; private set; } = default!;

    public string Country { get; private set; } = default!;

    public string PostalCode { get; private set; } = default!;

    private PostalAddress() { }

    public static PostalAddress Empty() => new();

    public PostalAddress(
        string addressLine1,
        string? addressLine2,
        string city,
        string state,
        string country,
        string postalCode)
    {
        if (string.IsNullOrWhiteSpace(addressLine1))
            throw new ArgumentException("Address line 1 is required.", nameof(addressLine1));

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City is required.", nameof(city));

        if (string.IsNullOrWhiteSpace(state))
            throw new ArgumentException("State is required.", nameof(state));

        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country is required.", nameof(country));

        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("Postal code is required.", nameof(postalCode));

        AddressLine1 = addressLine1;
        AddressLine2 = addressLine2;
        City = city;
        State = state;
        Country = country;
        PostalCode = postalCode;
    }
}

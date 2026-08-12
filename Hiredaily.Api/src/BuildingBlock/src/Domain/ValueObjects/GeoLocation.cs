using System.Globalization;

namespace Hiredaily.BuildingBlock.Domain.ValueObjects;

public record GeoLocation
{
    public string Lat { get; private set; } = default!;

    public string Long { get; private set; } = default!;

    private GeoLocation() { }

    public static GeoLocation Empty() => new();

    public GeoLocation(string lat, string longitude)
    {
        if (!TryParseCoordinate(lat, -90, 90, out _))
            throw new ArgumentException("Latitude must be a valid value between -90 and 90.", nameof(lat));

        if (!TryParseCoordinate(longitude, -180, 180, out _))
            throw new ArgumentException("Longitude must be a valid value between -180 and 180.", nameof(longitude));

        Lat = lat;
        Long = longitude;
    }

    private static bool TryParseCoordinate(string value, double minimum, double maximum, out double coordinate)
    {
        return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out coordinate)
            && coordinate >= minimum
            && coordinate <= maximum;
    }
}

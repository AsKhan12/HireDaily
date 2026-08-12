using System.Globalization;
using Hiredaily.Modules.Feed.Application.Common;

namespace Hiredaily.Modules.Feed.Infra.Repository;

public partial class CosmosJobFeedRepository
{
    public static class PartionKeyHashGenerator
    {
        private const string Base32 = "0123456789bcdefghjkmnpqrstuvwxyz";
        private const int DefaultPrecision = 7;

        public static string CreateFrom(Location location, int precision = DefaultPrecision)
        {
            if (precision <= 0)
                throw new ArgumentOutOfRangeException(nameof(precision), "Precision must be greater than zero.");

            var latitude = double.Parse(location.Lat, CultureInfo.InvariantCulture);
            var longitude = double.Parse(location.Long, CultureInfo.InvariantCulture);

            var latitudeRange = new[] { -90.0, 90.0 };
            var longitudeRange = new[] { -180.0, 180.0 };
            var hash = new char[precision];
            var isEvenBit = true;
            var bit = 0;
            var characterIndex = 0;
            var currentValue = 0;

            while (characterIndex < precision)
            {
                var range = isEvenBit ? longitudeRange : latitudeRange;
                var coordinate = isEvenBit ? longitude : latitude;
                var midpoint = (range[0] + range[1]) / 2;

                if (coordinate >= midpoint)
                {
                    currentValue = (currentValue << 1) + 1;
                    range[0] = midpoint;
                }
                else
                {
                    currentValue <<= 1;
                    range[1] = midpoint;
                }

                isEvenBit = !isEvenBit;

                if (++bit != 5)
                    continue;

                hash[characterIndex++] = Base32[currentValue];
                bit = 0;
                currentValue = 0;
            }

            return new string(hash);
        }

        public static string CreatePartitionKey(Location location)
        {
            ArgumentNullException.ThrowIfNull(location);
            return CreateFrom(location);
        }

        public static IReadOnlyList<string> GetNearbyBuckets(
            Location location,
            int radiusInBuckets = 1)
        {
            ArgumentNullException.ThrowIfNull(location);

            if (radiusInBuckets < 0)
                throw new ArgumentOutOfRangeException(nameof(radiusInBuckets), "Radius must be greater than or equal to zero.");

            var latitude = double.Parse(location.Lat, CultureInfo.InvariantCulture);
            var longitude = double.Parse(location.Long, CultureInfo.InvariantCulture);

            var buckets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            const double stepDegrees = 0.1;

            for (var latOffset = -radiusInBuckets; latOffset <= radiusInBuckets; latOffset++)
            {
                for (var lonOffset = -radiusInBuckets; lonOffset <= radiusInBuckets; lonOffset++)
                {
                    var candidateLat = Clamp(latitude + (latOffset * stepDegrees), -90d, 90d);
                    var candidateLon = Clamp(longitude + (lonOffset * stepDegrees), -180d, 180d);

                    var candidateLocation = new Location
                    {
                        Lat = candidateLat.ToString(CultureInfo.InvariantCulture),
                        Long = candidateLon.ToString(CultureInfo.InvariantCulture)
                    };

                    buckets.Add(CreatePartitionKey(candidateLocation));
                }
            }

            return buckets.ToArray();
        }

        private static double Clamp(double value, double min, double max)
        {
            return Math.Min(max, Math.Max(min, value));
        }

        private static string NormalizeSkillName(string name)
        {
            return name.Trim().ToLowerInvariant().Replace(' ', '_');
        }
    }
}

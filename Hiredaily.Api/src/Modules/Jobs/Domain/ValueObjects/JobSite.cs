using Hiredaily.BuildingBlock.Domain.ValueObjects;

namespace Hiredaily.Modules.Jobs.Domain.ValueObjects;

public record JobSite
{
    public GeoLocation Location {get; private set;}
    public PostalAddress Address {get; private set;}

    public JobSite(GeoLocation location, PostalAddress address)
    {
        Location = location;
        Address = address;
    }
    private JobSite()
    {
    }
}
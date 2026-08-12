
using Hiredaily.BuildingBlock.Application.Mediator.Requests;
using Hiredaily.Modules.Feed.Application.Common;

namespace Hiredaily.Modules.Feed.Application.Read;

public class JobFeedRequest : IRequest<IEnumerable<JobFeedResponse>>
{
    public Guid RequestId { get; }
    public DateTime RequestedAt { get; }
    public string RequestedBy { get; }
    public Location? Location {get;}
    public List<string>? Skills { get; }

    public JobFeedRequest(string requestedBy, Location? location, List<string>? skills)
    {
        RequestedBy = requestedBy;
        RequestedAt = DateTime.UtcNow;
        RequestId = Guid.NewGuid();
        Location = location;
        Skills = skills;
    }
}

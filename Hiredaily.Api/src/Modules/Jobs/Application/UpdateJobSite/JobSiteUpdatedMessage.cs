using Hiredaily.BuildingBlock.Application.Events;

namespace Hiredaily.Modules.Jobs.Application.UpdateJobSite;

public class JobSiteUpdatedMessage : IMessage<JobSiteUpdatedMessagePayload>
{
    public required JobSiteUpdatedMessagePayload EventData {get; init;}

    public DateTime CreatedAt {get; init;}

    public Guid EventId {get; init;}

    public string EventName => "job.site.updated";
}

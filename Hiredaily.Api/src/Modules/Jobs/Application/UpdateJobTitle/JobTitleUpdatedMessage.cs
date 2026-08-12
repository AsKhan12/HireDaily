using Hiredaily.BuildingBlock.Application.Events;

namespace Hiredaily.Modules.Jobs.Application.UpdateJobTitle;

public class JobTitleUpdatedMessage : IMessage<JobTitleMessagePayload>
{
    public required JobTitleMessagePayload EventData {get; init;}

    public DateTime CreatedAt {get; init;}

    public Guid EventId {get; init;}

    public string EventName => "job.title.updated";
}

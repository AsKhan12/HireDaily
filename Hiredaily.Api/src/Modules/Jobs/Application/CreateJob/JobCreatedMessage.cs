using Hiredaily.BuildingBlock.Application.Events;

namespace Hiredaily.Modules.Jobs.Application.CreateJob;

public class JobCreatedMessage : IMessage<JobCreatedMessagePayload>
{
    public required JobCreatedMessagePayload EventData { get; init; }

    public DateTime CreatedAt { get; init; }

    public Guid EventId { get; init; }

    public string EventName => "job.created";
}

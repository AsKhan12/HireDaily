using Hiredaily.BuildingBlock.Application.Events;

namespace Hiredaily.Modules.Jobs.Application.UpdateHourlyRate;

public class JobHourlyRateUpdatedMessage : IMessage<JobHourlyRateUpdatedPayload>
{
    public required JobHourlyRateUpdatedPayload EventData {get; init;}

    public DateTime CreatedAt {get; init;}

    public Guid EventId {get; init;}

    public string EventName => "job.hourly-rate.updated";
}
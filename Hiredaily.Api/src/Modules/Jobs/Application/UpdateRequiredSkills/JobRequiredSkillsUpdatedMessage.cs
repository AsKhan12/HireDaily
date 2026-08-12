using Hiredaily.BuildingBlock.Application.Events;

namespace Hiredaily.Modules.Jobs.Application.UpdateRequiredSkills;

public class JobRequiredSkillsUpdatedMessage : IMessage<JobSkillsUpdatedMessagePayload>
{
    public required JobSkillsUpdatedMessagePayload EventData { get; init; }

    public DateTime CreatedAt { get; init; }

    public Guid EventId { get; init; }

    public string EventName => "job.skills.updated";
}
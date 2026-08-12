using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Domain.Events;
using Hiredaily.BuildingBlock.Domain.ValueObjects;

namespace Hiredaily.Modules.Jobs.Domain.Events;

public record JobRequiredSkillsUpdatedEvent : IDomainEvent
{
    public JobId AggregateId { get; }
    public IReadOnlyList<Skill> RequiredSkills { get; }

    public JobRequiredSkillsUpdatedEvent(JobId aggregateId, IReadOnlyList<Skill> requiredSkills)
    {
        AggregateId = aggregateId;
        RequiredSkills = requiredSkills;
    }
}

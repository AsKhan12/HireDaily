using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Domain.Events;
using Hiredaily.BuildingBlock.Domain.ValueObjects;

namespace Hiredaily.Modules.Identity.Domain.User.Events;

public sealed record UserSkillsUpdatedEvent : IDomainEvent
{
    public UserId AggregateId {get;}
    public IReadOnlyList<Skill> Skills {get;}

    public UserSkillsUpdatedEvent(UserId aggregateId, List<Skill> skills)
    {
        AggregateId = aggregateId;
        Skills = skills;
    }
}
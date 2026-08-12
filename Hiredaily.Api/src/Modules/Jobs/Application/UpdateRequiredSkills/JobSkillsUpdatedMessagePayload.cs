using Hiredaily.BuildingBlock.Application.Events;
using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Domain.ValueObjects;

namespace Hiredaily.Modules.Jobs.Application.UpdateRequiredSkills;

public record JobSkillsUpdatedMessagePayload(JobId JobId, IReadOnlyList<Skill> RequiredSkills) : IMessagePayload;
using Hiredaily.BuildingBlock.Application.Events;
using Hiredaily.Modules.Feed.Application.Common;

namespace Hiredaily.Modules.Feed.Application.Write;
public record JobRequiredSkillsUpdatedMessage(JobId JobId, IReadOnlyList<Skill> RequiredSkills);
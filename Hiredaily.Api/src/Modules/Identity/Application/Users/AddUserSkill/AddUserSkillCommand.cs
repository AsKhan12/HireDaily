using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Application.Mediator.Requests;
using Hiredaily.Modules.Identity.Application.Users.Shared;

namespace Hiredaily.Modules.Identity.Application.Users.AddUserSkill;

public class AddUserSkillCommand : ICommand
{
    public Guid RequestId { get; set; }
    public DateTime RequestedAt { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public UserId UserId { get; set; } = default!;
    public SkillDto Skill { get; set; } = default!;
}

using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.Modules.Identity.Application.Users.Shared;
using Hiredaily.Modules.Identity.Domain.User.ValueObject;

namespace Hiredaily.Modules.Identity.Application.Users.GetUser;

public class GetUserResponse
{
    public UserId UserId { get; set; } = default!;
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public UserAddress Address { get; set; } = default!;
    public IReadOnlyList<SkillDto> Skills { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

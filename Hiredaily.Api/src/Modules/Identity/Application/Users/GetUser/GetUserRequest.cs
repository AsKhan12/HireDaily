using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Application.Mediator.Requests;

namespace Hiredaily.Modules.Identity.Application.Users.GetUser;

public class GetUserRequest : IRequest<GetUserResponse>
{
    public Guid RequestId { get; set; } = Guid.NewGuid();
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    public string RequestedBy { get; set; } = string.Empty;
    public UserId UserId { get; set; } = default!;
}

using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Application.Mediator.Requests;

namespace Hiredaily.Modules.Identity.Application.Users.UpdateUserName;

public class UpdateUserNameCommand : ICommand
{
    public Guid RequestId { get; set; }
    public DateTime RequestedAt { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public UserId UserId { get; set; } = default!;
    public string? UpdatedName { get; set; }
}

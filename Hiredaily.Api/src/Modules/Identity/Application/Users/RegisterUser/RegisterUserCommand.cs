using Hiredaily.BuildingBlock.Application.Mediator.Requests;

namespace Hiredaily.Modules.Identity.Application.Users.RegisterUser;

public class RegisterUserCommand : ICommand
{
    public Guid RequestId { get; set; }
    public DateTime RequestedAt { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

namespace Hiredaily.Modules.Identity.API.Features.Auth.RequestModels;

public class AuthenticatedUser
{
    public required string Username {get; init;}
    public required string Name {get; init;}
    public Guid UserId {get; init;}
    public string Role {get; init;} = "User";
}
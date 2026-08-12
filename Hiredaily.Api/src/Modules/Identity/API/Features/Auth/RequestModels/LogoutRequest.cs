namespace Hiredaily.Modules.Identity.API.Features.Auth.RequestModels;

public class LogoutRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}

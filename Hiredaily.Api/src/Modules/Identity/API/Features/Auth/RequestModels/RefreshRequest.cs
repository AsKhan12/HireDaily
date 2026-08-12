namespace Hiredaily.Modules.Identity.API.Features.Auth.RequestModels;

public class RefreshRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
namespace Hiredaily.Modules.Identity.API.Security;

public interface IJwtTokenService
{
    string GenerateToken(string id, string name, string email, string role);
}

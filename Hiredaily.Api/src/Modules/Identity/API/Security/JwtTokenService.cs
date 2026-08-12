using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Hiredaily.Modules.Identity.API.Security;

public class JwtTokenService : IJwtTokenService
{
    private readonly string key;
    private readonly string issuer;
    private readonly string audience;

    public JwtTokenService(IConfiguration configuration)
    {
        key = configuration["Jwt:Key"] ?? "CHANGE_THIS_SECRET";
        issuer = configuration["Jwt:Issuer"] ?? "hiredaily";
        audience = configuration["Jwt:Audience"] ?? "hiredaily_users";
    }

    public string GenerateToken(string id, string name, string email, string role)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, id),
            new Claim("name", name ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Email, email ?? string.Empty),
            new Claim(ClaimTypes.Role, role ?? "user"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

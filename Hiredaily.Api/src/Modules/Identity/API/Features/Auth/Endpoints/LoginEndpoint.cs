using Hiredaily.Modules.Identity.API.Features.Auth.RequestModels;
using Hiredaily.Modules.Identity.Application.Abstraction;
using Hiredaily.Modules.Identity.Application.Models;
using Hiredaily.Modules.Identity.Domain.Organization.Abstraction;
using Hiredaily.Modules.Identity.Domain.User.Abstraction;
using Hiredaily.Modules.Identity.API.Security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Security.Cryptography;
using System.Text;

namespace Hiredaily.Modules.Identity.API.Features.Auth.Endpoints;

public static partial class AuthRouteExtension
{
    public static RouteGroupBuilder MapLoginEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/login", HandleLogin);
        return group;
    }

    private static async Task<IResult> HandleLogin(
        LoginRequest request,
        IJwtTokenService jwtTokenService,
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository,
        IRefreshTokenStoreRepository refreshTokenStoreRepository,
        HttpResponse httpResponse,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return Results.BadRequest();

        // compute password hash using same SHA256 hex as registration
        string passwordHash;
        using (var sha = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(request.Password);
            var hash = sha.ComputeHash(bytes);
            passwordHash = Convert.ToHexString(hash);
        }

        // try user
        var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is not null && !string.IsNullOrEmpty(user.PasswordHash) && user.PasswordHash == passwordHash)
        {
            var userEmail = user.Username;
            var token = jwtTokenService.GenerateToken(user.Id.Value.ToString(), user.Name, userEmail, "User");

            // create refresh token store
            var refreshToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
            var refreshExpiry = DateTime.UtcNow.AddDays(30);
            var tokenStore = new RefreshTokenStore(Guid.NewGuid(), user.Id.Value, null, refreshToken, refreshExpiry);
            await refreshTokenStoreRepository.AddAsync(tokenStore, cancellationToken);
            await refreshTokenStoreRepository.SaveChangesAsync(cancellationToken);

            // set httponly SameSite cookie for refresh token
            httpResponse.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = false,
                Expires = refreshExpiry
            });

            var userDto = new AuthenticatedUser { UserId = user.Id.Value, Name = user.Name, Role = "User", Username = userEmail };
            return Results.Ok(new { token, refreshTokenExpiresAt = refreshExpiry, user = userDto });
        }

        // try organization
        var org = await organizationRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (org is not null && !string.IsNullOrEmpty(org.PasswordHash) && org.PasswordHash == passwordHash)
        {
            var orgEmail = org.Username ?? string.Empty;
            var token = jwtTokenService.GenerateToken(org.Id.Value.ToString(), org.Name, orgEmail, "Organization");

            var refreshToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
            var refreshExpiry = DateTime.UtcNow.AddDays(30);
            var tokenStore = new RefreshTokenStore(Guid.NewGuid(), null, org.Id.Value, refreshToken, refreshExpiry);
            await refreshTokenStoreRepository.AddAsync(tokenStore, cancellationToken);
            await refreshTokenStoreRepository.SaveChangesAsync(cancellationToken);

            httpResponse.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Strict,
                Secure = false,
                Expires = refreshExpiry
            });

            var orgDto = new AuthenticatedUser { UserId = org.Id.Value, Name = org.Name, Role = "Organization", Username = orgEmail };
            return Results.Ok(new { token, refreshTokenExpiresAt = refreshExpiry, user = orgDto });
        }

        return Results.Unauthorized();
    }
}

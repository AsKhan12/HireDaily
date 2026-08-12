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
using Hiredaily.BuildingBlock.Domain.EntityIds;
namespace Hiredaily.Modules.Identity.API.Features.Auth.Endpoints;

public static partial class AuthRouteExtension
{
    public static RouteGroupBuilder MapRefreshEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/refresh", HandleRefresh);
        return group;
    }

    private static async Task<IResult> HandleRefresh(
        RefreshRequest? request,
        IJwtTokenService jwtTokenService,
        IOrganizationRepository organizationRepository,
        IUserRepository userRepository,
        IRefreshTokenStoreRepository refreshTokenStoreRepository,
        HttpRequest httpRequest,
        HttpResponse httpResponse,
        CancellationToken cancellationToken = default)
    {
        var incoming = request?.RefreshToken;
        if (string.IsNullOrWhiteSpace(incoming))
        {
            // try cookie
            httpRequest.Cookies.TryGetValue("refreshToken", out var cookieToken);
            incoming = cookieToken;
        }

        if (string.IsNullOrWhiteSpace(incoming))
            return Results.BadRequest();

        // get refresh token store
        var tokenStore = await refreshTokenStoreRepository.GetByTokenAsync(incoming, cancellationToken);
        if (tokenStore is null || tokenStore.ExpiresAt <= DateTime.UtcNow)
            return Results.Unauthorized();

        // try user
        if (tokenStore.UserId.HasValue)
        {
            var user = await userRepository.GetByIdAsync(new UserId(tokenStore.UserId.Value), cancellationToken);
            if (user is not null)
            {
                var userEmail = user.Username ?? string.Empty;
                var token = jwtTokenService.GenerateToken(user.Id.Value.ToString(), user.Name, userEmail, "User");

                // delete old token and create new one
                await refreshTokenStoreRepository.DeleteAsync(tokenStore.Id, cancellationToken);

                var newRefresh = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
                var refreshExpiry = DateTime.UtcNow.AddDays(30);
                var newTokenStore = new RefreshTokenStore(Guid.NewGuid(), user.Id.Value, null, newRefresh, refreshExpiry);
                await refreshTokenStoreRepository.AddAsync(newTokenStore, cancellationToken);
                await refreshTokenStoreRepository.SaveChangesAsync(cancellationToken);

                httpResponse.Cookies.Append("refreshToken", newRefresh, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Secure = false,
                    Expires = refreshExpiry
                });

                var userDto = new AuthenticatedUser { UserId = user.Id.Value, Name = user.Name, Role = "User", Username = userEmail };
                return Results.Ok(new { token, refreshTokenExpiresAt = refreshExpiry, user = userDto });
            }
        }

        // try organization
        if (tokenStore.OrganizationId.HasValue)
        {
            var org = await organizationRepository.GetByIdAsync(new OrganizationId(tokenStore.OrganizationId.Value), cancellationToken);
            if (org is not null)
            {
                var orgEmail = org.Username ?? string.Empty;
                var token = jwtTokenService.GenerateToken(org.Id.Value.ToString(), org.Name, orgEmail, "Organization");

                // delete old token and create new one
                await refreshTokenStoreRepository.DeleteAsync(tokenStore.Id, cancellationToken);

                var newRefresh = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
                var refreshExpiry = DateTime.UtcNow.AddDays(30);
                var newTokenStore = new RefreshTokenStore(Guid.NewGuid(), null, org.Id.Value, newRefresh, refreshExpiry);
                await refreshTokenStoreRepository.AddAsync(newTokenStore, cancellationToken);
                await refreshTokenStoreRepository.SaveChangesAsync(cancellationToken);

                httpResponse.Cookies.Append("refreshToken", newRefresh, new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Secure = true,
                    Expires = refreshExpiry
                });

                var orgDto = new AuthenticatedUser { UserId = org.Id.Value, Name = org.Name, Role = "Organization", Username = orgEmail };
                return Results.Ok(new { token, refreshTokenExpiresAt = refreshExpiry, user = orgDto });
            }
        }

        return Results.Unauthorized();
    }

}
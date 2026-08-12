using Hiredaily.Modules.Identity.API.Features.Auth.RequestModels;
using Hiredaily.Modules.Identity.Application.Abstraction;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Hiredaily.Modules.Identity.API.Features.Auth.Endpoints;

public static partial class AuthRouteExtension
{
    public static RouteGroupBuilder MapLogoutEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/logout", HandleLogout);
        return group;
    }

    private static async Task<IResult> HandleLogout(
        LogoutRequest? logoutRequest,
        IRefreshTokenStoreRepository refreshTokenStoreRepository,
        HttpRequest httpRequest,
        CancellationToken cancellationToken = default)
    {
        var incoming = logoutRequest?.RefreshToken;
        if (string.IsNullOrWhiteSpace(incoming))
        {
            // try cookie
            httpRequest.Cookies.TryGetValue("refreshToken", out var cookieToken);
            incoming = cookieToken;
        }
        if(string.IsNullOrWhiteSpace(incoming))
        {
            return Results.BadRequest();
        }
        var tokenStore = await refreshTokenStoreRepository.GetByTokenAsync(incoming, cancellationToken);
        if(tokenStore is null)
        {
            return Results.BadRequest();
        }
        await refreshTokenStoreRepository.DeleteAsync(tokenStore.Id, cancellationToken);
        return Results.Ok();
    }
}
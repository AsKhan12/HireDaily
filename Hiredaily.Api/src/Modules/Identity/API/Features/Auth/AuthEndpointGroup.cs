using Hiredaily.Modules.Identity.API.Features.Auth.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;

namespace Hiredaily.Modules.Identity.API.Features.Auth;

public static class AuthEndpointGroup
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/auth")
            .WithMetadata(new TagsAttribute("Auth"))
            .MapLoginEndpoint()
            .MapRefreshEndpoint()
            .MapLogoutEndpoint();
    }
}

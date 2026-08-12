using Microsoft.AspNetCore.Builder;
using Hiredaily.Modules.Identity.API.Features.Organizations;
using Hiredaily.Modules.Identity.API.Features.Users;
using Hiredaily.Modules.Identity.API.Features.Auth;

namespace Hiredaily.Modules.Identity.API;

public static class IdentityModuleExtension
{
    public static void UseIdentityModule(this WebApplication app)
    {
        app.MapOrganizationEndpoints();
        app.MapUserEndpoints();
        app.MapAuthEndpoints();
    }
}

using Hiredaily.Modules.Identity.API.Features.Users.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Hiredaily.Modules.Identity.API.Features.Users;

public static class UserEndpointGroup
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/user");
        group.WithMetadata(new TagsAttribute("User"));
        group.MapRegisterUserEndpoint();
        group.MapGetUserEndpoint();
        group.MapUpdateUserNameEndpoint();
        group.MapAddUserSkillEndpoint();
        group.MapRemoveUserSkillEndpoint();
        group.MapUpdateUserAddressEndpoint();
    }
}

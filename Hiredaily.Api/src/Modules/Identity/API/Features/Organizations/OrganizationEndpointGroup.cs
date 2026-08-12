using Hiredaily.Modules.Identity.API.Features.Organizations.Endpoiints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Hiredaily.Modules.Identity.API.Features.Organizations;

public static class OrganizationEndpointGroup
{
    public static void MapOrganizationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/organization");
        group.WithMetadata(new TagsAttribute("Organization"));
        group.MapRegisterEndpoint();
        group.MapActivateEndpoint();
        group.MapSuspendEndpoint();
        group.MapUpdateEndpoint();
        group.MapGetOrganizationEndpoint();
    }
}

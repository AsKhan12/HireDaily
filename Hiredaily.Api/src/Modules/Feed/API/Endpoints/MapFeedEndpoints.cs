using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Hiredaily.Modules.Feed.API.Endpoints;

public static class FeedEndpointGroup
{
    public static void MapFeedEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/feed");
        group.WithMetadata(new TagsAttribute("Feed"));
        group.MapGetFeedEndpoint();
    }
}

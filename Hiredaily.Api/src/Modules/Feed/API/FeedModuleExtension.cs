using Hiredaily.Modules.Feed.API.Endpoints;
using Microsoft.AspNetCore.Builder;

namespace Hiredaily.Modules.Feed.API;

public static class FeedModuleExtension
{
    public static void UseFeedModule(this WebApplication app)
    {
        app.MapFeedEndpoints();
    }
}
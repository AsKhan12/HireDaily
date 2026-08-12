using Hiredaily.Modules.Jobs.API.Features.Jobs.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Hiredaily.Modules.Jobs.API.Endpoints;

public static class JobEndpointGroup
{
    public static void MapJobEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGroup("/job")
            .WithMetadata(new TagsAttribute("Job"))
            .MapCreateJobEndpoint()
            .MapGetJobEndpoint()
            .MapGetOrganizationJobEndpoint()
            .MapUpdateJobHourlyRateEndpoint()
            .MapUpdateJobSiteEndpoint()
            .MapUpdateJobRequiredSkillsEndpoint()
            .MapUpdateJobTitleEndpoint();
    }
}
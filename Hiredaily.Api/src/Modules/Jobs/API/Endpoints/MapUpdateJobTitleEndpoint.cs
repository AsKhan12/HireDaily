using Hiredaily.BuildingBlock.Application.Mediator;
using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.Modules.Jobs.API.InputRequestModels;
using Hiredaily.Modules.Jobs.Application.UpdateJobTitle;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Hiredaily.Modules.Jobs.API.Features.Jobs.Endpoints;

public static partial class RouteExtension
{
    public static RouteGroupBuilder MapUpdateJobTitleEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}/title", HandleUpdateJobtitle);
        return group;
    }

    private static async Task<IResult> HandleUpdateJobtitle(
        Guid id,
        UpdateJobTitleInput input,
        IMediatr mediatr,
        ILogger<UpdateJobTitleCommand> logger,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateJobTitleCommand
        {
            Title = input.Title,
            JobId = new JobId(id),
            RequestedBy = "organization"
        };
        var result = await mediatr.Send(command, cancellationToken);
        return ToHttpResult(result, logger);
    }
}
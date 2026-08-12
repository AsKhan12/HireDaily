using Hiredaily.BuildingBlock.Application.Mediator;
using Hiredaily.Modules.Feed.API.InputRequestModels;
using Hiredaily.Modules.Feed.Application.Read;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using IMediatorResult = Hiredaily.BuildingBlock.Application.Mediator.src.Results.IResult;

namespace Hiredaily.Modules.Feed.API.Endpoints;

public static partial class RouteExtension
{
    public static RouteGroupBuilder MapGetFeedEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/search", HandleGetFeed);
        return group;
    }

    private static async Task<IResult> HandleGetFeed(
        [FromBody] JobFeedRequestInput request,
        IMediatr mediatr,
        ILogger<JobFeedRequest> logger,
        CancellationToken cancellationToken = default)
    {
        var result = await mediatr.Send<JobFeedRequest, IEnumerable<JobFeedResponse>>(
            new JobFeedRequest("", request.Location, request.Skills),
            cancellationToken);

        if (result.IsSuccess)
            return Results.Ok(result.Response);

        LogFailure(result, logger);
        return !result.ValidationResult.IsValid
            ? Results.BadRequest()
            : Results.InternalServerError();
    }
    private static void LogFailure<TLogger>(
        IMediatorResult result,
        ILogger<TLogger> logger)
    {
        logger.LogError("{Error}", result.Error);
        if (!result.ValidationResult.IsValid)
            logger.LogError("{ValidationErrors}", result.ValidationResult.ToString());
    }
}

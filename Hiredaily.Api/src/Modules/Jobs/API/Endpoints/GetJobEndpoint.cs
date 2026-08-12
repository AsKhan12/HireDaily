using Hiredaily.BuildingBlock.Application.Mediator;
using IMediatorResult = Hiredaily.BuildingBlock.Application.Mediator.src.Results.IResult;
using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.Modules.Jobs.Application.GetJob;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Hiredaily.Modules.Jobs.API.Features.Jobs.Endpoints;

public static partial class RouteExtension
{
    public static RouteGroupBuilder MapGetJobEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", HandleGetJob);
        return group;
    }

    private static async Task<IResult> HandleGetJob(
        Guid id,
        IMediatr mediatr,
        ILogger<GetJobRequest> logger,
        CancellationToken cancellationToken = default)
    {
        var result = await mediatr.Send<GetJobRequest, GetJobResponse>(
            new GetJobRequest { JobId = new JobId(id) },
            cancellationToken);

        if (result.IsSuccess)
            return Results.Ok(result.Response);

        LogFailure(result, logger);
        return !result.ValidationResult.IsValid
            ? Results.BadRequest()
            : Results.InternalServerError();
    }

    private static IResult ToHttpResult<TCommand>(
        IMediatorResult result,
        ILogger<TCommand> logger)
    {
        if (result.IsSuccess)
            return Results.Ok();

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

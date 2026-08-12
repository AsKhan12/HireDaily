using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Hiredaily.Modules.Identity.API.Features.Users.Endpoints;

public static partial class UserRouteExtension
{
    private static IResult ToHttpResult<TCommand>(
        Hiredaily.BuildingBlock.Application.Mediator.src.Results.IResult result,
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
        Hiredaily.BuildingBlock.Application.Mediator.src.Results.IResult result,
        ILogger<TLogger> logger)
    {
        logger.LogError("{Error}", result.Error);
        if (!result.ValidationResult.IsValid)
            logger.LogError("{ValidationErrors}", result.ValidationResult.ToString());
    }
}


using Hiredaily.BuildingBlock.Application.Mediator;
using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.Modules.Identity.API.Features.Users.RequestModels;
using Hiredaily.Modules.Identity.Application.Users.GetUser;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Hiredaily.Modules.Identity.API.Features.Users.Endpoints;

public static partial class UserRouteExtension
{
    public static RouteGroupBuilder MapGetUserEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", HandleGetUser);
        return group;
    }

    private static async Task<IResult> HandleGetUser(
        [AsParameters] GetUserInput input,
        IMediatr mediatr,
        ILogger<GetUserRequest> logger,
        CancellationToken cancellationToken = default)
    {
        var request = new GetUserRequest
        {
            RequestId = Guid.NewGuid(),
            RequestedAt = DateTime.UtcNow,
            RequestedBy = "Applicant",
            UserId = new UserId(input.Id)
        };

        var result = await mediatr.Send<GetUserRequest, GetUserResponse>(
            request,
            cancellationToken);

        if (result.IsSuccess)
            return Results.Ok(result.Response);

        LogFailure(result, logger);
        return !result.ValidationResult.IsValid
            ? Results.BadRequest()
            : Results.InternalServerError();
    }
}

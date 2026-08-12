using Hiredaily.BuildingBlock.Application.Mediator;
using Hiredaily.Modules.Identity.API.Features.Organizations.RequestModels;
using Hiredaily.Modules.Identity.Application.Organizations.SuspendOrganization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

public static partial class RouteExtension
{
    public static RouteGroupBuilder MapSuspendEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("/suspend", HandleSuspension);
        return group;
    }

    private static async Task<IResult> HandleSuspension(
        SuspendOrganizationInput input,
        IMediatr mediatr,
        ILogger<SuspendOrganizationCommand> logger,
        CancellationToken cancellationToken = default)
    {
        var command  = new SuspendOrganizationCommand
        {
            OrganizationId = input.OrganizationId,
            RequestedAt = DateTime.UtcNow,
            RequestedBy = "admin",
            RequestId = Guid.NewGuid()
        };
        var result = await mediatr.Send(command, cancellationToken);
        if (result.IsSuccess)
            return Results.Ok();
        logger.LogError("{Error}", result.Error);
        if (!result.ValidationResult.IsValid)
        {
            logger.LogError("{validationErrors}", result.ValidationResult.ToString());
            return Results.BadRequest();
        }
        return Results.InternalServerError();

    }
}
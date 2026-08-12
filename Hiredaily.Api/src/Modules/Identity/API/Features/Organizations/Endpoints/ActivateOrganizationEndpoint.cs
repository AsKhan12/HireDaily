using Hiredaily.BuildingBlock.Application.Mediator;
using Hiredaily.Modules.Identity.API.Features.Organizations.RequestModels;
using Hiredaily.Modules.Identity.Application.Organizations.ActivateOrganization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Hiredaily.Modules.Identity.API.Features.Organizations.Endpoiints;

public static partial class RouteExtension
{
    public static RouteGroupBuilder MapActivateEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/activate", HandleActivation);
        return group;
    }

    private static async Task<IResult> HandleActivation(
        ActivateOrganizationInput input, 
        IMediatr mediatr, 
        ILogger<ActivateOrganizationCommand> logger,
        CancellationToken cancellationToken = default)
    {
        var command  = new ActivateOrganizationCommand
        {
            OrganizationId = input.OrganizationId,
            RequestedAt = DateTime.UtcNow,
            RequestedBy = "admin",
            RequestId = Guid.NewGuid()
        };
        var result =  await mediatr.Send(command, cancellationToken);
        if(result.IsSuccess)
          return Results.Ok();
        logger.LogError("{Error}", result.Error);
        if(!result.ValidationResult.IsValid)
        {
            logger.LogError("{validationErrors}", result.ValidationResult.ToString());
            return Results.BadRequest();
        }
        return Results.InternalServerError();

    }
}

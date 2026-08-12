using Hiredaily.BuildingBlock.Application.Mediator;
using Hiredaily.Modules.Identity.API.Features.Organizations.RequestModels;
using Hiredaily.Modules.Identity.Application.Organizations.RegisterOrganization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Hiredaily.Modules.Identity.API.Features.Organizations.Endpoiints;

public static partial class RouteExtension
{
    public static RouteGroupBuilder MapRegisterEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", HandleRegistration);
        return group;
    }

    private static async Task<IResult> HandleRegistration(
        RegisterOrganizationInput request, 
        IMediatr mediatr, 
        ILogger<RegisterOrganizationCommand> logger,
        CancellationToken cancellationToken = default)
    {
        var command = new RegisterOrganizationCommand
        {
            RequestId = Guid.NewGuid(),
            RequestedAt = DateTime.UtcNow,
            RequestedBy = "Organization",

            Name = request.Name,
            Email = request.Email,
            Password = request.Password
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

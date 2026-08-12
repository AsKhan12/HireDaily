using Hiredaily.BuildingBlock.Application.Mediator;
using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.Modules.Identity.Application.Organizations.GetOrganization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Hiredaily.Modules.Identity.API.Features.Organizations.Endpoiints;

public static partial class RouteExtension
{
    public static RouteGroupBuilder MapGetOrganizationEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/{id:guid}", HandleGetOrganization);
        return group;
    }
    private static async Task<IResult> HandleGetOrganization(
        Guid id,
        IMediatr mediatr,
        ILogger<GetOrganizationRequest> logger,
        CancellationToken cancellationToken = default)
    {
        var result = await mediatr.Send<GetOrganizationRequest, GetOrganizationResponse>(
                                                new GetOrganizationRequest() 
                                                { 
                                                    OrganizationId = new OrganizationId(id) 
                                                },
                                                cancellationToken);
        if (result.IsSuccess)
            return Results.Ok(result.Response);
        logger.LogError("{Error}", result.Error);
        if (!result.ValidationResult.IsValid)
        {
            logger.LogError("{validationErrors}", result.ValidationResult.ToString());
            return Results.BadRequest();
        }
        return Results.InternalServerError();

    }
}

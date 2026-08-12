using Hiredaily.BuildingBlock.Application.Mediator;
using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.Modules.Jobs.Application.GetOrganizationJob;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Hiredaily.Modules.Jobs.API.Features.Jobs.Endpoints;

public static partial class RouteExtension
{
    public static RouteGroupBuilder MapGetOrganizationJobEndpoint(this RouteGroupBuilder group)
    {
        group.MapGet("/organization/{organizationId:guid}", HandleGetorganizationJob);
        return group;
    }

    private static async Task<IResult> HandleGetorganizationJob(
        Guid organizationId,
        IMediatr mediatr,
        ILogger<GetOrganizatoinJobRequest> logger,
        CancellationToken cancellationToken = default)
    {
        var result = await mediatr.Send<GetOrganizatoinJobRequest, GetOrganizationJobResponse>(
            new GetOrganizatoinJobRequest { OrganizationId = new OrganizationId(organizationId) },
            cancellationToken);

        if (result.IsSuccess)
            return Results.Ok(result.Response);

        LogFailure(result, logger);
        return !result.ValidationResult.IsValid
            ? Results.BadRequest()
            : Results.InternalServerError();
    }
}

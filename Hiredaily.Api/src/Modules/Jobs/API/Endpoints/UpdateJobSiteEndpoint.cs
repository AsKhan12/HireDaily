using Hiredaily.BuildingBlock.Application.Mediator;
using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.Modules.Jobs.API.InputRequestModels;
using Hiredaily.Modules.Jobs.Application.UpdateJobSite;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Hiredaily.Modules.Jobs.API.Features.Jobs.Endpoints;

public static partial class RouteExtension
{
    public static RouteGroupBuilder MapUpdateJobSiteEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}/site", HandleUpdateJobSite);
        return group;
    }

    private static async Task<IResult> HandleUpdateJobSite(
        Guid id,
        UpdateJobSiteInput input,
        IMediatr mediatr,
        ILogger<UpdateJobSiteCommand> logger,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateJobSiteCommand
        {
            AddressLine1 = input.AddressLine1,
            AddressLine2 = input.AddressLine2,
            City = input.City,
            Country = input.Country,
            JobId = new JobId(id),
            Latitude = input.Latitude,
            Longitude = input.Longitude,
            PostalCode = input.PostalCode,
            RequestedAt = DateTime.UtcNow,
            RequestedBy = "organization",
            RequestId = Guid.NewGuid(),
            State = input.State
        };
        var result = await mediatr.Send(command, cancellationToken);
        return ToHttpResult(result, logger);
    }
}
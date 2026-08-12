using Hiredaily.BuildingBlock.Application.Mediator;
using Hiredaily.Modules.Jobs.API.InputRequestModels;
using Hiredaily.Modules.Jobs.Application.CreateJob;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Hiredaily.Modules.Jobs.API.Features.Jobs.Endpoints;

public static partial class RouteExtension
{
    public static RouteGroupBuilder MapCreateJobEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", HandleCreateJob);
        return group;
    }

    private static async Task<IResult> HandleCreateJob(
        CreateJobInput input,
        IMediatr mediatr,
        ILogger<CreateJobCommand> logger,
        CancellationToken cancellationToken = default)
    {
        var command = new CreateJobCommand
        {
            Title = input.Title,
            RequestId = Guid.NewGuid(),
            RequestedAt = DateTime.UtcNow,
            RequestedBy = "organization",
            OrganizationId = input.OrganizationId,
            HourlyRateAmount = input.HourlyRateAmount,
            HourlyRateCurrency = input.HourlyRateCurrency,
            Latitude = input.Latitude,
            Longitude = input.Longitude,
            AddressLine1 = input.AddressLine1,
            AddressLine2 = input.AddressLine2,
            City = input.City,
            State = input.State,
            Country = input.Country,
            PostalCode = input.PostalCode,
            RequiredSkills = input.RequiredSkills
        };

        var result = await mediatr.Send(command, cancellationToken);
        return ToHttpResult(result, logger);
    }
}

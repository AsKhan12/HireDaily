using Hiredaily.BuildingBlock.Application.Mediator;
using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.Modules.Jobs.API.InputRequestModels;
using Hiredaily.Modules.Jobs.Application.UpdateHourlyRate;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Hiredaily.Modules.Jobs.API.Features.Jobs.Endpoints;

public static partial class RouteExtension
{
    public static RouteGroupBuilder MapUpdateJobHourlyRateEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}/hourly-rate", HandleUpdateJobHourlyRate);
        return group;
    }

    private static async Task<IResult> HandleUpdateJobHourlyRate(
        Guid id,
        UpdateJobHourlyRateInput input,
        IMediatr mediatr,
        ILogger<UpdateJobHourlyRateCommand> logger,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateJobHourlyRateCommand
        {
            RequestedAt = DateTime.UtcNow,
            RequestedBy = "organization",
            JobId = new JobId(id),
            Amount = input.Amount,
            Currency = input.Currency,
            RequestId = Guid.NewGuid()
        };
        var result = await mediatr.Send(command, cancellationToken);
        return ToHttpResult(result, logger);
    }
}

using Hiredaily.BuildingBlock.Application.Mediator;
using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.Modules.Jobs.API.InputRequestModels;
using Hiredaily.Modules.Jobs.Application.UpdateRequiredSkills;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Hiredaily.Modules.Jobs.API.Features.Jobs.Endpoints;

public static partial class RouteExtension
{
    public static RouteGroupBuilder MapUpdateJobRequiredSkillsEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}/required-skills", HandleUpdateJobRequiredSkills);
        return group;
    }

    private static async Task<IResult> HandleUpdateJobRequiredSkills(
        Guid id,
        UpdateJobRequiredSkillInput input,
        IMediatr mediatr,
        ILogger<UpdateJobRequiredSkillsCommand> logger,
        CancellationToken cancellationToken = default)
    {
        var command  = new UpdateJobRequiredSkillsCommand
        {
            JobId = new JobId(id),
            RequestedAt = DateTime.UtcNow,
            RequestedBy = "organization",
            RequestId = Guid.NewGuid(),
            RequiredSkills = input.RequiredSkills
        };
        var result = await mediatr.Send(command, cancellationToken);
        return ToHttpResult(result, logger);
    }
}

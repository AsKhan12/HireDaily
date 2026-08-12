using Hiredaily.BuildingBlock.Application.Mediator;
using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.Modules.Identity.API.Features.Users.RequestModels;
using Hiredaily.Modules.Identity.Application.Users.AddUserSkill;
using Hiredaily.Modules.Identity.Application.Users.Shared;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Hiredaily.Modules.Identity.API.Features.Users.Endpoints;

public static partial class UserRouteExtension
{
    public static RouteGroupBuilder MapAddUserSkillEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}/skills", HandleAddUserSkill);
        return group;
    }

    private static async Task<IResult> HandleAddUserSkill(
        Guid id,
        UserSkillInput input,
        IMediatr mediatr,
        ILogger<AddUserSkillCommand> logger,
        CancellationToken cancellationToken = default)
    {
        var command = new AddUserSkillCommand
        {
            RequestId = Guid.NewGuid(),
            RequestedAt = DateTime.UtcNow,
            RequestedBy = "Applicant",
            UserId = new UserId(id),
            Skill = new SkillDto
            {
                Name = input.Name,
                Field = input.Field,
                Description = input.Description,
                SkillLevel = input.SkillLevel
            }
        };

        var result = await mediatr.Send(command, cancellationToken);
        return ToHttpResult(result, logger);
    }
}

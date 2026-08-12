using Hiredaily.BuildingBlock.Application.Mediator;
using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.Modules.Identity.Application.Users.UpdateUserName;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Hiredaily.Modules.Identity.API.Features.Users.Endpoints;

public static partial class UserRouteExtension
{
    public static RouteGroupBuilder MapUpdateUserNameEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}/name", HandleUpdateUserName);
        return group;
    }

    private static async Task<IResult> HandleUpdateUserName(
        Guid id,
        string name,
        IMediatr mediatr,
        ILogger<UpdateUserNameCommand> logger,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateUserNameCommand
        {
            RequestId = Guid.NewGuid(),
            UpdatedName = name,
            UserId = new UserId(id),
            RequestedAt = DateTime.UtcNow,
            RequestedBy = id.ToString()
        };
        var result = await mediatr.Send(command, cancellationToken);
        return ToHttpResult(result, logger);
    }
}

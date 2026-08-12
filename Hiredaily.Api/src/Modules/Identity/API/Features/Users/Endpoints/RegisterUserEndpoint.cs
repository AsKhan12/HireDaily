using Hiredaily.BuildingBlock.Application.Mediator;
using Hiredaily.Modules.Identity.API.Features.Users.RequestModels;
using Hiredaily.Modules.Identity.Application.Users.RegisterUser;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Hiredaily.Modules.Identity.API.Features.Users.Endpoints;

public static partial class UserRouteExtension
{
    public static RouteGroupBuilder MapRegisterUserEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/", HandleRegistration);
        return group;
    }

    private static async Task<IResult> HandleRegistration(
        RegisterUserInput input,
        IMediatr mediatr,
        ILogger<RegisterUserCommand> logger,
        HttpContext httpContext,
        CancellationToken cancellationToken = default)
    {
        var command = new RegisterUserCommand
        {
            RequestId = Guid.NewGuid(),
            RequestedAt = DateTime.UtcNow,
            RequestedBy = "Applicant",
            Name = input.Name,
            Email = input.Email,
            Password = input.Password
        };

        var result = await mediatr.Send(command, cancellationToken);
        return ToHttpResult(result, logger);
    }
}

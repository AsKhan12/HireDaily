using Hiredaily.BuildingBlock.Application.Mediator;
using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.Modules.Identity.API.Features.Users.RequestModels;
using Hiredaily.Modules.Identity.Application.Users.UpdateUserAddress;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Hiredaily.Modules.Identity.API.Features.Users.Endpoints;

public static partial class UserRouteExtension
{
    public static RouteGroupBuilder MapUpdateUserAddressEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("/{id:guid}/address", HandleUpdateUserAddress);
        return group;
    }

    private static async Task<IResult> HandleUpdateUserAddress(
        Guid id,
        UpdateUserAddressInput input,
        IMediatr mediatr,
        ILogger<UpdateUserAddressCommand> logger,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateUserAddressCommand
        {
            RequestId = Guid.NewGuid(),
            RequestedAt = DateTime.UtcNow,
            RequestedBy = "Applicant",
            UserId = new UserId(id),
            Email = input.Email,
            Phone = input.Phone,
            AddressLine1 = input.AddressLine1,
            AddressLine2 = input.AddressLine2,
            City = input.City,
            State = input.State,
            Country = input.Country,
            PostalCode = input.PostalCode,
            Latitude = input.Latitude,
            Longitude = input.Longitude
        };

        var result = await mediatr.Send(command, cancellationToken);
        return ToHttpResult(result, logger);
    }
}

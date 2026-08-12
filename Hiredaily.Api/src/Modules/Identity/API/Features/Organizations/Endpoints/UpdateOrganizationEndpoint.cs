using Hiredaily.BuildingBlock.Domain.ValueObjects;
using Hiredaily.BuildingBlock.Application.Mediator;
using Hiredaily.Modules.Identity.API.Features.Organizations.RequestModels;
using Hiredaily.Modules.Identity.Application.Organizations.UpdateOrganization;
using Hiredaily.Modules.Identity.Domain.Organization.ValueObjects;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Hiredaily.BuildingBlock.Domain.EntityIds;

public static partial class RouteExtension
{
    public static RouteGroupBuilder MapUpdateEndpoint(this RouteGroupBuilder group)
    {
        group.MapPut("/", HandleUpdation);
        return group;
    }

    private static async Task<IResult> HandleUpdation(
        UpdateOrganizationInput request,
        IMediatr mediatr,
        ILogger<UpdateOrganizationCommand> logger,
        CancellationToken cancellationToken = default)
    {
        var command = new UpdateOrganizationCommand
        {
            RequestId = request.RequestId,
            RequestedAt = request.RequestedAt,
            RequestedBy = request.RequestedBy,
            OrganizationId = new OrganizationId(request.OrganizationId.Value),
            UpdatedName = request.UpdatedName,
            UpdatedDescription = request.UpdatedDescription
        };

        if (request.UpdatedAddress is not null)
        {
            var loc = request.UpdatedAddress.Location is null
                ? GeoLocation.Empty()
                : new GeoLocation(request.UpdatedAddress.Location.Lat, request.UpdatedAddress.Location.Long);

            var postal = request.UpdatedAddress.PostalAddress is null
                ? PostalAddress.Empty()
                : new PostalAddress(
                    request.UpdatedAddress.PostalAddress.AddressLine1,
                    request.UpdatedAddress.PostalAddress.AddressLine2,
                    request.UpdatedAddress.PostalAddress.City,
                    request.UpdatedAddress.PostalAddress.State,
                    request.UpdatedAddress.PostalAddress.Country,
                    request.UpdatedAddress.PostalAddress.PostalCode);

            var contact = request.UpdatedAddress.ContactDetails is null
                ? OrganizationContactDetails.Empty()
                : new OrganizationContactDetails(
                    request.UpdatedAddress.ContactDetails.Email,
                    request.UpdatedAddress.ContactDetails.Phone,
                    request.UpdatedAddress.ContactDetails.WebsiteUrl);

            command.UpdatedAddress = new OrganizationAddress(loc, postal, contact);
        }

        var result = await mediatr.Send(command, cancellationToken);
        if(result.IsSuccess)
          return Results.Ok();
        logger.LogError("{Error}", result.Error);
        if(!result.ValidationResult.IsValid)
        {
            logger.LogError("{validationErrors}", result.ValidationResult.ToString());
            return Results.BadRequest();
        }
        return Results.InternalServerError();

    }
}

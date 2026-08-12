using Hiredaily.BuildingBlock.Application.Mediator.Handlers;
using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;
using Hiredaily.BuildingBlock.Application.Mediator.src.Results;
using Hiredaily.Modules.Identity.Domain.Organization.Abstraction;

namespace Hiredaily.Modules.Identity.Application.Organizations.GetOrganization;

public class GetOrganizationRequestHandler(IOrganizationRepository organizationRepository) : IRequestHandler<GetOrganizationRequest, GetOrganizationResponse>
{
    public async Task<IResult<GetOrganizationResponse>> Handle(GetOrganizationRequest request, CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.GetByIdAsync( request.OrganizationId );
        if ( organization == null )
           return Result<GetOrganizationResponse>.Failure( ValidationResult.Valid(), "not found!" );
        var response = new GetOrganizationResponse
        {
            OrganizationId = organization.Id,
            OrganizationName = organization.Name,
            OrganizationDescription = organization.Description,
            Address = organization.Address,
            Description = organization.Description,
            Status = organization.Status,
            Username = organization.Username,
        };
        return Result<GetOrganizationResponse>.Success(response);
    }
}
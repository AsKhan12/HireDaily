using Hiredaily.BuildingBlock.Application.Mediator.Handlers;
using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;
using Hiredaily.BuildingBlock.Application.Mediator.src.Results;
using Hiredaily.Modules.Identity.Domain.Organization.Abstraction;

namespace Hiredaily.Modules.Identity.Application.Organizations.ActivateOrganization;

public class ActivateOrganizationCommandHandler(IOrganizationRepository organizationRepository) : ICommandHandler<ActivateOrganizationCommand>
{
    public async Task<IResult> Handle(ActivateOrganizationCommand command, CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.GetByIdAsync(command.OrganizationId);
        if (organization == null)
          return Result.Failure(ValidationResult.Valid(), "not found!");
        organization.Activate();
        await organizationRepository.SaveChangesAsync();
        return Result.Success();
    }
}
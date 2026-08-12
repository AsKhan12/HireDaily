using Hiredaily.BuildingBlock.Application.Mediator.Handlers;
using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;
using Hiredaily.BuildingBlock.Application.Mediator.src.Results;
using Hiredaily.Modules.Identity.Domain.Organization.Abstraction;

namespace Hiredaily.Modules.Identity.Application.Organizations.UpdateOrganization;

public class UpdateOrganizationCommandHandler(IOrganizationRepository organizationRepository) 
    : ICommandHandler<UpdateOrganizationCommand>
{
    public async Task<IResult> Handle(UpdateOrganizationCommand command, CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.GetByIdAsync(command.OrganizationId, cancellationToken);
        if(organization is null)
          return Result.Failure(ValidationResult.Valid(), "Not found!");
        if(!string.IsNullOrWhiteSpace(command.UpdatedName))
            organization.Rename(command.UpdatedName!);
        if(!string.IsNullOrWhiteSpace(command.UpdatedDescription))
            organization.UpdateDescription(command.UpdatedDescription);
        if(command.UpdatedAddress is not null)
            organization.ChangeAddress(command.UpdatedAddress);

        await organizationRepository.SaveChangesAsync();
        return Result.Success();
    }
}

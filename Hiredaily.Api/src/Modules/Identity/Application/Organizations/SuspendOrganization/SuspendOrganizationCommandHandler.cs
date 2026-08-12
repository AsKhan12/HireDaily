using Hiredaily.BuildingBlock.Application.Mediator.Handlers;
using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;
using Hiredaily.BuildingBlock.Application.Mediator.src.Results;
using Hiredaily.Modules.Identity.Domain.Organization.Abstraction;

namespace Hiredaily.Modules.Identity.Application.Organizations.SuspendOrganization;

public class SuspendOrganizationCommandHandler(IOrganizationRepository organizationRepository) : ICommandHandler<SuspendOrganizationCommand>
{
    public async Task<IResult> Handle(SuspendOrganizationCommand request, CancellationToken cancellationToken)
    {
        var organization = await organizationRepository.GetByIdAsync(request.OrganizationId);
        if (organization == null)
           return Result.Failure(ValidationResult.Valid(), "not found!");
        organization.Suspend();
        await organizationRepository.SaveChangesAsync();
        return Result.Success();
    }
}
using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;

namespace Hiredaily.Modules.Identity.Application.Organizations.SuspendOrganization;

public class SuspendOrganizationCommandValidator : IValidator<SuspendOrganizationCommand>
{
    public Task<ValidationResult> ValidateAsync(
        SuspendOrganizationCommand request,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        ValidatorHelpers.AddRequestErrors(errors, request);
        ValidatorHelpers.AddRequired(
            errors,
            request.OrganizationId is null || request.OrganizationId.Value == Guid.Empty,
            nameof(request.OrganizationId),
            "Organization id is required.");

        return Task.FromResult(ValidatorHelpers.ToResult(errors));
    }
}

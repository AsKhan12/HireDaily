using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;

namespace Hiredaily.Modules.Identity.Application.Organizations.UpdateOrganization;

public class UpdateOrganizationCommandValidator : IValidator<UpdateOrganizationCommand>
{
    public Task<ValidationResult> ValidateAsync(
        UpdateOrganizationCommand request,
        CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        ValidatorHelpers.AddRequestErrors(errors, request);
        ValidatorHelpers.AddRequired(
            errors,
            request.OrganizationId is null || request.OrganizationId.Value == Guid.Empty,
            nameof(request.OrganizationId),
            "Organization id is required.");
        ValidatorHelpers.AddRequired(
            errors,
            request.UpdatedName is not null && string.IsNullOrWhiteSpace(request.UpdatedName),
            nameof(request.UpdatedName),
            "Organization name cannot be empty.");
        ValidatorHelpers.AddRequired(
            errors,
            request.UpdatedDescription is not null && string.IsNullOrWhiteSpace(request.UpdatedDescription),
            nameof(request.UpdatedDescription),
            "Organization description cannot be empty.");
        ValidatorHelpers.AddRequired(
            errors,
            request.UpdatedName is null &&
            request.UpdatedDescription is null &&
            request.UpdatedAddress is null,
            nameof(request.UpdatedName),
            "An updated name, description, or address is required.");

        return Task.FromResult(ValidatorHelpers.ToResult(errors));
    }
}

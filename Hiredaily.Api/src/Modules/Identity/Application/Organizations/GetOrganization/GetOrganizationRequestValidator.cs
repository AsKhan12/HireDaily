using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;

namespace Hiredaily.Modules.Identity.Application.Organizations.GetOrganization;

public class GetOrganizationRequestValidator : IValidator<GetOrganizationRequest>
{
    public Task<ValidationResult> ValidateAsync(GetOrganizationRequest request, CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        ValidatorHelpers.AddRequired(errors, request.OrganizationId is null || request.OrganizationId.Value == Guid.Empty, nameof(request.OrganizationId), "Org id is required.");
        return Task.FromResult(ValidatorHelpers.ToResult(errors));
    }
}

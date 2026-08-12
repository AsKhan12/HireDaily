using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;

namespace Hiredaily.Modules.Jobs.Application.GetOrganizationJob;

public class GetOrganizationJobRequestValidator : IValidator<GetOrganizatoinJobRequest>
{
    public Task<ValidationResult> ValidateAsync(GetOrganizatoinJobRequest request, CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        ValidatorHelpers.AddRequired(errors, request.OrganizationId is null || request.OrganizationId.Value == Guid.Empty, nameof(request.OrganizationId), "Organization id is required.");
        return Task.FromResult(ValidatorHelpers.ToResult(errors));
    }
}
using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;

namespace Hiredaily.Modules.Jobs.Application.UpdateJobSite;

public class UpdateJobSiteCommandValidator : IValidator<UpdateJobSiteCommand>
{
    public Task<ValidationResult> ValidateAsync(UpdateJobSiteCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        ValidatorHelpers.AddRequestErrors(errors, request);
        ValidatorHelpers.AddRequired(errors, request.JobId is null || request.JobId.Value == Guid.Empty, nameof(request.JobId), "Job id is required.");
        ValidatorHelpers.AddRequired(errors, string.IsNullOrWhiteSpace(request.AddressLine1), nameof(request.AddressLine1), "Address line 1 is required.");
        ValidatorHelpers.AddRequired(errors, string.IsNullOrWhiteSpace(request.City), nameof(request.City), "City is required.");
        ValidatorHelpers.AddRequired(errors, string.IsNullOrWhiteSpace(request.State), nameof(request.State), "State is required.");
        ValidatorHelpers.AddRequired(errors, string.IsNullOrWhiteSpace(request.Country), nameof(request.Country), "Country is required.");
        ValidatorHelpers.AddRequired(errors, string.IsNullOrWhiteSpace(request.PostalCode), nameof(request.PostalCode), "Postal code is required.");
        return Task.FromResult(ValidatorHelpers.ToResult(errors));
    }
}

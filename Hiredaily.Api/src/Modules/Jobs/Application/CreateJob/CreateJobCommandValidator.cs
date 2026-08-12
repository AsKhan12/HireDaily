using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;

namespace Hiredaily.Modules.Jobs.Application.CreateJob;

public class CreateJobCommandValidator : IValidator<CreateJobCommand>
{
    public Task<ValidationResult> ValidateAsync(CreateJobCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        ValidatorHelpers.AddRequestErrors(errors, request);
        ValidatorHelpers.AddRequired(errors, request.OrganizationId == Guid.Empty, nameof(request.OrganizationId), "Organization id is required.");
        ValidatorHelpers.AddRequired(errors, string.IsNullOrWhiteSpace(request.Title), nameof(request.Title), "Title is required.");
        ValidatorHelpers.AddRequired(errors, request.HourlyRateAmount < 0, nameof(request.HourlyRateAmount), "Hourly rate amount cannot be negative.");
        ValidatorHelpers.AddRequired(errors, string.IsNullOrWhiteSpace(request.HourlyRateCurrency), nameof(request.HourlyRateCurrency), "Hourly rate currency is required.");
        ValidatorHelpers.AddRequired(errors, string.IsNullOrWhiteSpace(request.AddressLine1), nameof(request.AddressLine1), "Address line 1 is required.");
        ValidatorHelpers.AddRequired(errors, string.IsNullOrWhiteSpace(request.City), nameof(request.City), "City is required.");
        ValidatorHelpers.AddRequired(errors, string.IsNullOrWhiteSpace(request.State), nameof(request.State), "State is required.");
        ValidatorHelpers.AddRequired(errors, string.IsNullOrWhiteSpace(request.Country), nameof(request.Country), "Country is required.");
        ValidatorHelpers.AddRequired(errors, string.IsNullOrWhiteSpace(request.PostalCode), nameof(request.PostalCode), "Postal code is required.");

        return Task.FromResult(ValidatorHelpers.ToResult(errors));
    }
}

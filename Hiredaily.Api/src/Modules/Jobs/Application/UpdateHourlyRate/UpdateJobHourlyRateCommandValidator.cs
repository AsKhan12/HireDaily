using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;

namespace Hiredaily.Modules.Jobs.Application.UpdateHourlyRate;

public class UpdateJobHourlyRateCommandValidator : IValidator<UpdateJobHourlyRateCommand>
{
    public Task<ValidationResult> ValidateAsync(UpdateJobHourlyRateCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        ValidatorHelpers.AddRequestErrors(errors, request);
        ValidatorHelpers.AddRequired(errors, request.JobId is null || request.JobId.Value == Guid.Empty, nameof(request.JobId), "Job id is required.");
        ValidatorHelpers.AddRequired(errors, request.Amount < 0, nameof(request.Amount), "Amount cannot be negative.");
        ValidatorHelpers.AddRequired(errors, string.IsNullOrWhiteSpace(request.Currency), nameof(request.Currency), "Currency is required.");
        return Task.FromResult(ValidatorHelpers.ToResult(errors));
    }
}

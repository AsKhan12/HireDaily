using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;

namespace Hiredaily.Modules.Jobs.Application.GetJob;

public class GetJobRequestValidator : IValidator<GetJobRequest>
{
    public Task<ValidationResult> ValidateAsync(GetJobRequest request, CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        ValidatorHelpers.AddRequired(errors, request.JobId is null || request.JobId.Value == Guid.Empty, nameof(request.JobId), "Job id is required.");
        return Task.FromResult(ValidatorHelpers.ToResult(errors));
    }
}

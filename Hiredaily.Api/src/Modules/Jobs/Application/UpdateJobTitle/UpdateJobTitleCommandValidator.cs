using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;

namespace Hiredaily.Modules.Jobs.Application.UpdateJobTitle;

public class UpdateJobTitleCommandValidator : IValidator<UpdateJobTitleCommand>
{
    public Task<ValidationResult> ValidateAsync(UpdateJobTitleCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        ValidatorHelpers.AddRequestErrors(errors, request);
        ValidatorHelpers.AddRequired(errors, request.JobId is null || request.JobId.Value == Guid.Empty, nameof(request.JobId), "Job id is required.");
        return Task.FromResult(ValidatorHelpers.ToResult(errors));
    }
}
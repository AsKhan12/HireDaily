using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;

namespace Hiredaily.Modules.Jobs.Application.UpdateRequiredSkills;

public class UpdateJobRequiredSkillsCommandValidator : IValidator<UpdateJobRequiredSkillsCommand>
{
    public Task<ValidationResult> ValidateAsync(UpdateJobRequiredSkillsCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        ValidatorHelpers.AddRequestErrors(errors, request);
        ValidatorHelpers.AddRequired(errors, request.JobId is null || request.JobId.Value == Guid.Empty, nameof(request.JobId), "Job id is required.");
        ValidatorHelpers.AddRequired(errors, request.RequiredSkills is null, nameof(request.RequiredSkills), "Required skills are required.");
        return Task.FromResult(ValidatorHelpers.ToResult(errors));
    }
}

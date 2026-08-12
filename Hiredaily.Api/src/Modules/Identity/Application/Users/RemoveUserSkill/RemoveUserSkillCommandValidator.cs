using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;

namespace Hiredaily.Modules.Identity.Application.Users.RemoveUserSkill;

public class RemoveUserSkillCommandValidator : IValidator<RemoveUserSkillCommand>
{
    public Task<ValidationResult> ValidateAsync(RemoveUserSkillCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        ValidatorHelpers.AddRequestErrors(errors, request);
        ValidatorHelpers.AddRequired(errors, request.UserId is null || request.UserId.Value == Guid.Empty, nameof(request.UserId), "User id is required.");
        ValidatorHelpers.AddRequired(errors, request.Skill is null, nameof(request.Skill), "Skill is required.");
        return Task.FromResult(ValidatorHelpers.ToResult(errors));
    }
}

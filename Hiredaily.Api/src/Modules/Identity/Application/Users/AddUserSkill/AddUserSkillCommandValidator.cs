using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;

namespace Hiredaily.Modules.Identity.Application.Users.AddUserSkill;

public class AddUserSkillCommandValidator : IValidator<AddUserSkillCommand>
{
    public Task<ValidationResult> ValidateAsync(AddUserSkillCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        ValidatorHelpers.AddRequestErrors(errors, request);
        ValidatorHelpers.AddRequired(errors, request.UserId is null || request.UserId.Value == Guid.Empty, nameof(request.UserId), "User id is required.");
        ValidatorHelpers.AddRequired(errors, request.Skill is null, nameof(request.Skill), "Skill is required.");
        return Task.FromResult(ValidatorHelpers.ToResult(errors));
    }
}

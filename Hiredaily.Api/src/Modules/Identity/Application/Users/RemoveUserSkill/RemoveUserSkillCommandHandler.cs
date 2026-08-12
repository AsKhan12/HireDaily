using Hiredaily.BuildingBlock.Domain.ValueObjects;
using Hiredaily.BuildingBlock.Application.Mediator.Handlers;
using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;
using Hiredaily.BuildingBlock.Application.Mediator.src.Results;
using Hiredaily.Modules.Identity.Application.Users.Shared;
using Hiredaily.Modules.Identity.Domain.User.Abstraction;

namespace Hiredaily.Modules.Identity.Application.Users.RemoveUserSkill;

public class RemoveUserSkillCommandHandler(IUserRepository userRepository) : ICommandHandler<RemoveUserSkillCommand>
{
    public async Task<IResult> Handle(RemoveUserSkillCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
            return Result.Failure(ValidationResult.Valid(), "not found!");

        user.RemoveSkill(ToSkill(command.Skill));
        await userRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static Skill ToSkill(SkillDto skill)
    {
        return new Skill(skill.Name, skill.Field, skill.Description, skill.SkillLevel);
    }
}

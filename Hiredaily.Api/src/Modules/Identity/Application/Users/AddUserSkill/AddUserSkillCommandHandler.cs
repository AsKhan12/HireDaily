using Hiredaily.BuildingBlock.Domain.ValueObjects;
using Hiredaily.BuildingBlock.Application.Mediator.Handlers;
using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;
using Hiredaily.BuildingBlock.Application.Mediator.src.Results;
using Hiredaily.Modules.Identity.Application.Users.Shared;
using Hiredaily.Modules.Identity.Domain.User.Abstraction;

namespace Hiredaily.Modules.Identity.Application.Users.AddUserSkill;

public class AddUserSkillCommandHandler(IUserRepository userRepository) : ICommandHandler<AddUserSkillCommand>
{
    public async Task<IResult> Handle(AddUserSkillCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
            return Result.Failure(ValidationResult.Valid(), "not found!");

        user.AddSkill(ToSkill(command.Skill));
        await userRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    private static Skill ToSkill(SkillDto skill)
    {
        return new Skill(skill.Name, skill.Field, skill.Description, skill.SkillLevel);
    }
}

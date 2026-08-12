using Hiredaily.BuildingBlock.Application.Mediator.Handlers;
using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;
using Hiredaily.BuildingBlock.Application.Mediator.src.Results;
using Hiredaily.Modules.Identity.Domain.User.Abstraction;

namespace Hiredaily.Modules.Identity.Application.Users.UpdateUserName;

public class UpdateUserNameCommandHandler(IUserRepository userRepository) : ICommandHandler<UpdateUserNameCommand>
{
    public async Task<IResult> Handle(UpdateUserNameCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
            return Result.Failure(ValidationResult.Valid(), "not found!");

        user.UpdateName(command.UpdatedName!);

        await userRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

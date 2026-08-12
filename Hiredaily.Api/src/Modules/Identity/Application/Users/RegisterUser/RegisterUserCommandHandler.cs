using Hiredaily.BuildingBlock.Application.Mediator.Handlers;
using Hiredaily.BuildingBlock.Application.Mediator.src.Results;
using Hiredaily.Modules.Identity.Domain.User;
using Hiredaily.Modules.Identity.Domain.User.Abstraction;
using System.Security.Cryptography;
using System.Text;

namespace Hiredaily.Modules.Identity.Application.Users.RegisterUser;

public class RegisterUserCommandHandler(IUserRepository userRepository) : ICommandHandler<RegisterUserCommand>
{
    public async Task<IResult> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var passwordHash = string.Empty;
        if (!string.IsNullOrWhiteSpace(command.Password))
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(command.Password);
            var hash = sha.ComputeHash(bytes);
            passwordHash = Convert.ToHexString(hash);
        }

        var user = new User(
            name: command.Name,
            username: command.Email,
            passwordHash: passwordHash);

        await userRepository.AddAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

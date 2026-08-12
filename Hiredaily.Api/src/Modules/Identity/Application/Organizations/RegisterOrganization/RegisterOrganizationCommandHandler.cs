using Hiredaily.BuildingBlock.Application.Mediator.Handlers;
using Hiredaily.BuildingBlock.Application.Mediator.src.Results;
using Hiredaily.Modules.Identity.Domain.Organization;
using Hiredaily.Modules.Identity.Domain.Organization.Abstraction;
using System.Security.Cryptography;
using System.Text;

namespace Hiredaily.Modules.Identity.Application.Organizations.RegisterOrganization;

public class RegisterOrganizationCommandHandler(IOrganizationRepository organizationRepository) 
    : ICommandHandler<RegisterOrganizationCommand>
{
    public async Task<IResult> Handle(RegisterOrganizationCommand command, CancellationToken cancellationToken = default)
    {
        var passwordHash = string.Empty;
        if (!string.IsNullOrWhiteSpace(command.Password))
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(command.Password);
            var hash = sha.ComputeHash(bytes);
            passwordHash = Convert.ToHexString(hash);
        }

        var organization = new Organization(
            name: command.Name,
            username: command.Email,
            passwordHash: passwordHash
        );

        await organizationRepository.AddAsync(organization, cancellationToken);
        await organizationRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

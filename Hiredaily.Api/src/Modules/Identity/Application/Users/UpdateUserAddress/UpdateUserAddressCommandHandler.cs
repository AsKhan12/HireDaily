using Hiredaily.BuildingBlock.Domain.ValueObjects;
using Hiredaily.BuildingBlock.Application.Mediator.Handlers;
using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;
using Hiredaily.BuildingBlock.Application.Mediator.src.Results;
using Hiredaily.Modules.Identity.Domain.User.Abstraction;
using Hiredaily.Modules.Identity.Domain.User.ValueObject;

namespace Hiredaily.Modules.Identity.Application.Users.UpdateUserAddress;

public class UpdateUserAddressCommandHandler(IUserRepository userRepository) : ICommandHandler<UpdateUserAddressCommand>
{
    public async Task<IResult> Handle(UpdateUserAddressCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
            return Result.Failure(ValidationResult.Valid(), "not found!");

        user.UpdateAddress(new UserAddress(
            new GeoLocation(command.Latitude, command.Longitude),
            new PostalAddress(
                command.AddressLine1,
                command.AddressLine2,
                command.City,
                command.State,
                command.Country,
                command.PostalCode),
            new UserContactDetails(command.Phone, command.Email)));

        await userRepository.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

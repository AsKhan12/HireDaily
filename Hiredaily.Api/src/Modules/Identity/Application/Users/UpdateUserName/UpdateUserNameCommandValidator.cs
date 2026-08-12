using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;

namespace Hiredaily.Modules.Identity.Application.Users.UpdateUserName;

public class UpdateUserNameCommandValidator : IValidator<UpdateUserNameCommand>
{
    public Task<ValidationResult> ValidateAsync(UpdateUserNameCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        ValidatorHelpers.AddRequestErrors(errors, request);
        ValidatorHelpers.AddRequired(errors, request.UserId is null || request.UserId.Value == Guid.Empty, nameof(request.UserId), "User id is required.");
        ValidatorHelpers.AddRequired(errors, string.IsNullOrWhiteSpace(request.UpdatedName), nameof(request.UpdatedName), "User name is required.");
        return Task.FromResult(ValidatorHelpers.ToResult(errors));
    }
}

using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;

namespace Hiredaily.Modules.Identity.Application.Users.GetUser;

public class GetUserRequestValidator : IValidator<GetUserRequest>
{
    public Task<ValidationResult> ValidateAsync(GetUserRequest request, CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        ValidatorHelpers.AddRequired(errors, request.UserId is null || request.UserId.Value == Guid.Empty, nameof(request.UserId), "User id is required.");
        return Task.FromResult(ValidatorHelpers.ToResult(errors));
    }
}

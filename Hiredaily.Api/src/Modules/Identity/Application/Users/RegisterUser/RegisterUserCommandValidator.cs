using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;

namespace Hiredaily.Modules.Identity.Application.Users.RegisterUser;

public class RegisterUserCommandValidator : IValidator<RegisterUserCommand>
{
    public Task<ValidationResult> ValidateAsync(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        ValidatorHelpers.AddRequestErrors(errors, request);
        ValidatorHelpers.AddRequired(errors, string.IsNullOrWhiteSpace(request.Name), nameof(request.Name), "Name is required.");
        ValidatorHelpers.AddRequired(errors, string.IsNullOrWhiteSpace(request.Email), nameof(request.Email), "Email is required.");
        ValidatorHelpers.AddRequired(errors, string.IsNullOrWhiteSpace(request.Password), nameof(request.Password), "Password is required.");
        if (!string.IsNullOrWhiteSpace(request.Password) && request.Password.Length < 6)
            ValidatorHelpers.AddRequired(errors, true, nameof(request.Password), "Password must be at least 6 characters.");

        if (!string.IsNullOrWhiteSpace(request.Email) && !request.Email.Contains('@', StringComparison.Ordinal))
            ValidatorHelpers.AddRequired(errors, true, nameof(request.Email), "Email must be valid.");

        return Task.FromResult(ValidatorHelpers.ToResult(errors));
    }
}

using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;

namespace Hiredaily.Modules.Identity.Application.Users.UpdateUserAddress;

public class UpdateUserAddressCommandValidator : IValidator<UpdateUserAddressCommand>
{
    public Task<ValidationResult> ValidateAsync(UpdateUserAddressCommand request, CancellationToken cancellationToken)
    {
        var errors = new List<ValidationError>();
        ValidatorHelpers.AddRequestErrors(errors, request);
        ValidatorHelpers.AddRequired(errors, request.UserId is null || request.UserId.Value == Guid.Empty, nameof(request.UserId), "User id is required.");
        ValidatorHelpers.AddRequired(errors, string.IsNullOrWhiteSpace(request.Email), nameof(request.Email), "Email is required.");
        ValidatorHelpers.AddRequired(errors, string.IsNullOrWhiteSpace(request.Phone), nameof(request.Phone), "Phone is required.");
        ValidatorHelpers.AddRequired(errors, string.IsNullOrWhiteSpace(request.AddressLine1), nameof(request.AddressLine1), "Address line 1 is required.");
        ValidatorHelpers.AddRequired(errors, string.IsNullOrWhiteSpace(request.City), nameof(request.City), "City is required.");
        ValidatorHelpers.AddRequired(errors, string.IsNullOrWhiteSpace(request.State), nameof(request.State), "State is required.");
        ValidatorHelpers.AddRequired(errors, string.IsNullOrWhiteSpace(request.Country), nameof(request.Country), "Country is required.");
        ValidatorHelpers.AddRequired(errors, string.IsNullOrWhiteSpace(request.PostalCode), nameof(request.PostalCode), "Postal code is required.");
        return Task.FromResult(ValidatorHelpers.ToResult(errors));
    }
}

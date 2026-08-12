using System.Globalization;
using System.Text.RegularExpressions;
using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;

namespace Hiredaily.Modules.Identity.Application.Organizations.RegisterOrganization;

public sealed partial class RegisterOrganizationCommandValidator : IValidator<RegisterOrganizationCommand>
{
    public Task<ValidationResult> ValidateAsync(RegisterOrganizationCommand request, CancellationToken cancellationToken = default)
    {
        var errors = new List<ValidationError>();

        ValidatorHelpers.AddRequired(errors, request.RequestId == Guid.Empty, nameof(request.RequestId), "Request id is required.");
        ValidatorHelpers.AddRequired(errors, request.RequestedAt == default, nameof(request.RequestedAt), "Requested at is required.");
        ValidatorHelpers.AddRequired(errors, string.IsNullOrWhiteSpace(request.RequestedBy), nameof(request.RequestedBy), "Requested by is required.");
        ValidatorHelpers.AddRequired(errors, string.IsNullOrWhiteSpace(request.Name), nameof(request.Name), "Organization name is required.");
        ValidatorHelpers.AddRequired(errors, string.IsNullOrWhiteSpace(request.Email), nameof(request.Email), "Email is required.");
        
        if (!string.IsNullOrWhiteSpace(request.Password) && request.Password.Length < 6)
            ValidatorHelpers.AddError(errors, nameof(request.Password), "Password must be at least 6 characters.");

        if (!string.IsNullOrWhiteSpace(request.Email) && !EmailRegex.IsMatch(request.Email))
            ValidatorHelpers.AddError(errors, nameof(request.Email), "Email must be valid.");

        var result = errors.Count == 0
            ? ValidationResult.Valid()
            : ValidationResult.InValid(errors);

        return Task.FromResult(result);
    }
    private static bool TryParseCoordinate(string value, double minimum, double maximum)
    {
        return double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var coordinate)
            && coordinate >= minimum
            && coordinate <= maximum;
    }

    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private static readonly Regex PhoneRegex = new(
        @"^\+?[1-9]\d{7,14}$",
        RegexOptions.CultureInvariant | RegexOptions.Compiled);
}

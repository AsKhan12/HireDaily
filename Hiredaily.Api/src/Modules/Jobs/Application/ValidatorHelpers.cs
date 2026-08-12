using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;
using Hiredaily.BuildingBlock.Application.Mediator.Requests;

namespace Hiredaily.Modules.Jobs.Application;

internal static class ValidatorHelpers
{
    public static void AddRequestErrors(List<ValidationError> errors, ICommand request)
    {
        AddRequired(errors, request.RequestId == Guid.Empty, nameof(request.RequestId), "Request id is required.");
        AddRequired(errors, request.RequestedAt == default, nameof(request.RequestedAt), "Requested at is required.");
        AddRequired(errors, string.IsNullOrWhiteSpace(request.RequestedBy), nameof(request.RequestedBy), "Requested by is required.");
    }

    public static void AddRequired(List<ValidationError> errors, bool condition, string propertyName, string errorMessage)
    {
        if (!condition)
            return;

        errors.Add(new ValidationError
        {
            PropertyName = propertyName,
            ErrorMessage = errorMessage
        });
    }

    public static ValidationResult ToResult(IReadOnlyList<ValidationError> errors)
    {
        return errors.Count == 0
            ? ValidationResult.Valid()
            : ValidationResult.InValid(errors);
    }
}

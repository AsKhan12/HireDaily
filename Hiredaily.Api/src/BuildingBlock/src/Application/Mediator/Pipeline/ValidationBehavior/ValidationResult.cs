namespace Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;

public class ValidationResult
{
    public bool IsValid { get; private set; }

    public IReadOnlyList<ValidationError>? Errors { get; private set; }

    private ValidationResult(bool isValid, IReadOnlyList<ValidationError>? errors = null)
    {
        IsValid = isValid;
        if (errors is not null)
            Errors = errors;
    }

    public static ValidationResult Valid() => new(true);
    public static ValidationResult InValid(IReadOnlyList<ValidationError> errors) => new(false, errors);

    public override string ToString()
    {
        return Errors is not null 
                ? string.Join(", ", Errors.Select(e => $"{e.PropertyName}: {e.ErrorMessage}"))
                : "";
    }
}

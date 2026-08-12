namespace Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;

public sealed class ValidationError
{
    public required string PropertyName { get; init; }

    public required string ErrorMessage { get; init; }
}

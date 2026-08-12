using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;

namespace Hiredaily.BuildingBlock.Application.Mediator.src.Results;
public interface IResult
{
    bool IsSuccess { get; }
    string? Error { get; }
    ValidationResult ValidationResult{ get; }

}

public interface IResult<T> : IResult where T : class
{
    T? Response { get; }
}

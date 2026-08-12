using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;

namespace Hiredaily.BuildingBlock.Application.Mediator.src.Results;

public class Result : IResult
{
    public bool IsSuccess { get; private set; } = false;
    public string? Error { get; private set; }
    public ValidationResult ValidationResult { get; private set; }

    protected Result(ValidationResult validationResult, string error)
    {
        ValidationResult = validationResult;
        Error = error;
    }
    protected Result()
    {
        ValidationResult = ValidationResult.Valid();
        IsSuccess = true;
    }

    public static Result Success()
    {
        return new Result();
    }

    public static Result Failure(ValidationResult validationResult, string error)
    {
        return new Result(validationResult, error);
    }    

}

public class Result<T> : Result, IResult<T> where T : class
{
    public T? Response { get; private set; }

    private Result(T response): base()
    {
        Response = response;
    }

    public static Result<T> Success(T response)
    {
        return new Result<T>(response);
    }

    protected Result(ValidationResult validationResult, string error) 
        : base(validationResult, error)
    {
    }

    public static new Result<T> Failure(ValidationResult validationResult, string error)
    {
        return new Result<T>(validationResult, error);
    }
}
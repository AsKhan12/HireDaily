using Hiredaily.BuildingBlock.Application.Mediator.Requests;
using Hiredaily.BuildingBlock.Application.Mediator.src.Results;
using Microsoft.Extensions.DependencyInjection;

namespace Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;

public class ValidationPipelineBehavior(
    IServiceScopeFactory factory) : IPipelineBehavior
{
    public IPipelineBehavior? Next { get; set; }

    public async Task<IResult> Start<TCommand>(TCommand command, CancellationToken cancellationToken = default)
    where TCommand : ICommand
    {
        using var scope  = factory.CreateScope();
        var validator = scope.ServiceProvider.GetRequiredService<IValidator<TCommand>>();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure(validationResult, "validation errors!");
        }
        if (Next == null)
            return Result.Success();
        return await Next.Start(command, cancellationToken);
    }
}

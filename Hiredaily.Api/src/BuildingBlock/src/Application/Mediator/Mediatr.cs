namespace Hiredaily.BuildingBlock.Application.Mediator;

using Hiredaily.BuildingBlock.Application.Mediator.Handlers;
using Hiredaily.BuildingBlock.Application.Mediator.Pipeline;
using Hiredaily.BuildingBlock.Application.Mediator.Requests;
using Hiredaily.BuildingBlock.Application.Mediator.src.Results;
using Microsoft.Extensions.DependencyInjection;

public class Mediatr(IServiceScopeFactory factory, IServiceProvider serviceProvider) : IMediatr
{
    public async Task<IResult<TResponse>> Send<TRequest, TResponse>(
        TRequest request, 
        CancellationToken cancellationToken = default)
            where TRequest : IRequest<TResponse>
            where TResponse : class
    {
        var pipelineResult = await RunPipeline(request);
        if (!pipelineResult.IsSuccess)
            return Result<TResponse>.Failure(pipelineResult.ValidationResult, pipelineResult.Error!);
        using var scope = factory.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        return await handler.Handle(request, cancellationToken);
    }

    public async Task<IResult> Send<TCommand>(
            TCommand request, 
            CancellationToken cancellationToken = default
        ) where TCommand : ICommand
    {
        var pipelineResult = await RunPipeline(request);
        if (!pipelineResult.IsSuccess)
            return pipelineResult;
        using var scope = factory.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<TCommand>>();
        return await handler.Handle(request, cancellationToken);
    }
    private async Task<IResult> RunPipeline<TCommand>(TCommand command) where TCommand : ICommand
    {
        var pipeline = serviceProvider.GetRequiredService<PipelineStartup>();
        return await pipeline.Run(command);
    }
}

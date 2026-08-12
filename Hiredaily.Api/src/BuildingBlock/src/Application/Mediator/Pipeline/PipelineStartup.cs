namespace Hiredaily.BuildingBlock.Application.Mediator.Pipeline;

using Hiredaily.BuildingBlock.Application.Mediator.Requests;
using Hiredaily.BuildingBlock.Application.Mediator.src.Results;

public class PipelineStartup
{
    private readonly IBehaviorCollection _collection;

    public PipelineStartup(IBehaviorCollection collection, IBehaviorConfiguration configuration)
    {
        configuration.Configure(collection);
        _collection = collection;
    }

    public async Task<IResult> Run<TCommand>(TCommand request, CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        if (_collection.First is null)
            return Result.Success();
        var first = _collection.First;
        return await first.Start(request, cancellationToken);
    }
}

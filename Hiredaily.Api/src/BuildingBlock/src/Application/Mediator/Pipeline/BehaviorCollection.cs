using Microsoft.Extensions.DependencyInjection;

namespace Hiredaily.BuildingBlock.Application.Mediator.Pipeline;

public class BehaviorCollection(IServiceProvider serviceProvider) : IBehaviorCollection
{
    private IPipelineBehavior? _current;
    private IPipelineBehavior? _first;

    public IPipelineBehavior? First => _first;

    public void Add<TBehaviour>() where TBehaviour : IPipelineBehavior
    {
        var behaviour = serviceProvider.GetRequiredService<TBehaviour>();
        if (_first is null)
        {
            _first = behaviour;
            _current = _first;
        }
        else
        {
            _current?.Next = behaviour;
            _current = behaviour;
        }
    }
}

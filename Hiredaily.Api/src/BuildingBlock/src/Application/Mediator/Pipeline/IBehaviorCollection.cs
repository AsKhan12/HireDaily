namespace Hiredaily.BuildingBlock.Application.Mediator.Pipeline;

public interface IBehaviorCollection
{
    void Add<TBehaviour>()where TBehaviour : IPipelineBehavior;
    IPipelineBehavior? First { get; }
}

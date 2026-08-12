using Hiredaily.BuildingBlock.Application.Mediator.Requests;
using Hiredaily.BuildingBlock.Application.Mediator.src.Results;

namespace Hiredaily.BuildingBlock.Application.Mediator.Pipeline;

public interface IPipelineBehavior
{
    IPipelineBehavior? Next {get; set;}
    Task<IResult> Start<TCommand>(TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommand;
}

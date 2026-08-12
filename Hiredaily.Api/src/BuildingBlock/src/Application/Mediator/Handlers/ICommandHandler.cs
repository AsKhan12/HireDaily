using Hiredaily.BuildingBlock.Application.Mediator.Requests;
using Hiredaily.BuildingBlock.Application.Mediator.src.Results;

namespace Hiredaily.BuildingBlock.Application.Mediator.Handlers;

public interface ICommandHandler<TRequest>
    where TRequest : ICommand
{
    Task<IResult> Handle(TRequest request, CancellationToken cancellationToken);
}

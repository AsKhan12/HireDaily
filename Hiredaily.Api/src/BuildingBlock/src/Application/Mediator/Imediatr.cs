using Hiredaily.BuildingBlock.Application.Mediator.Requests;
using Hiredaily.BuildingBlock.Application.Mediator.src.Results;

namespace Hiredaily.BuildingBlock.Application.Mediator;

public interface IMediatr
{
    Task<IResult<TResponse>> Send<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
        where TRequest : IRequest<TResponse>
        where TResponse : class;
    
    Task<IResult> Send<TCommand>(TCommand request, CancellationToken cancellationToken)
        where TCommand : ICommand;
}

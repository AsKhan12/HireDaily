using Hiredaily.BuildingBlock.Application.Mediator.Requests;
using Hiredaily.BuildingBlock.Application.Mediator.src.Results;

namespace Hiredaily.BuildingBlock.Application.Mediator.Handlers;
public interface IRequestHandler<TRequest, TResponse> 
    where TRequest: IRequest<TResponse>
    where TResponse : class
{
    Task<IResult<TResponse>> Handle(TRequest request, CancellationToken cancellationToken);
}

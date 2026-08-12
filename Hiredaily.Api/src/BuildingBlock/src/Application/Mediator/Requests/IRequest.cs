namespace Hiredaily.BuildingBlock.Application.Mediator.Requests;
public interface IRequest<T> : ICommand where T : class
{
}

using Hiredaily.BuildingBlock.Domain.Events;

namespace Hiredaily.BuildingBlock.Application.Events;

public interface IDomainEventHandler<T> where T: class, IDomainEvent
{
    Task Handle(T evt);
}

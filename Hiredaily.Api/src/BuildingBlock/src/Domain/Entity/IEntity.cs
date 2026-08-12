using Hiredaily.BuildingBlock.Domain.Events;

namespace Hiredaily.BuildingBlock.Domain.Entity;

public interface IEntity
{
    DateTime CreatedAt { get; }
    DateTime? UpdatedAt { get; }
    IReadOnlyList<IDomainEvent> Events { get; }
    void ClearEvents();
}
public interface IEntity<TId> : IEntity where TId : IIdentity
{
    TId Id { get; }
}

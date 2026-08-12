namespace Hiredaily.BuildingBlock.Application.Events;

public interface IMessage
{
    DateTime CreatedAt { get; }
    Guid EventId { get; }
    string EventName { get; }
}
public interface IMessage<T> : IMessage where T : class, IMessagePayload
{
    T EventData { get; }
}

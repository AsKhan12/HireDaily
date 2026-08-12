namespace Hiredaily.BuildingBlock.Application.Events;

// To deserialize and get event name and id without knowing the exact type of IIntegrationEvent
public class IntegrationEvent : IMessage
{
    public DateTime CreatedAt { get; init; }

    public Guid EventId { get; init; }

    public required string EventName { get; init; }
}
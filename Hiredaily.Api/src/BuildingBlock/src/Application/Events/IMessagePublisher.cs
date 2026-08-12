namespace Hiredaily.BuildingBlock.Application.Events;

public interface IMessagePublisher
{
    public Task PublishAsync(MessageEnvelope message, CancellationToken cancellationToken = default);
} 
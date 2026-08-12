using Hiredaily.BuildingBlock.Application.Events;

namespace Hiredaily.Modules.Jobs.Application.Outbox;

public interface IOutboxRepository
{
    public Task<IReadOnlyList<OutboxMessage>> GetUnPublishedMessages(CancellationToken cancellationToken);

    public Task MarkMessagesPublishedAsync(IEnumerable<Guid> messageIds, CancellationToken cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken);

    public void AddMessage<IT, T>(IT message) where IT: class, IMessage<T> where T : class, IMessagePayload; 
}

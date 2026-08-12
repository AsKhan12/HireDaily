using System.Text.Json;
using Hiredaily.Modules.Jobs.Application.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Hiredaily.Modules.Jobs.Infra.Persistence.SQL.Outbox;

public class OutboxRepository(JobsDbContext dbContext) : IOutboxRepository
{
    public async Task<IReadOnlyList<OutboxMessage>> GetUnPublishedMessages(CancellationToken cancellationToken)
    {
        return await dbContext.OutboxMessages
           .Where(message => message.PublishedAt == null)
           .OrderBy(message => message.OccurredAt)
           .ToListAsync(cancellationToken: cancellationToken);
    }
    public async Task MarkMessagesPublishedAsync(IEnumerable<Guid> messageIds, CancellationToken cancellationToken)
    {
        await dbContext
                .OutboxMessages
                .Where(x => messageIds.Contains(x.Id))
                .ExecuteUpdateAsync(s => s
                       .SetProperty(x => x.PublishedAt, DateTime.UtcNow), cancellationToken: cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    void IOutboxRepository.AddMessage<IT, T>(IT message)
    {
        var outbox = new OutboxMessage
        {
            Id = message.EventId,
            EventType = message.EventName,
            Payload = JsonSerializer.Serialize(message.EventData),
            OccurredAt = message.CreatedAt
        };
        dbContext.OutboxMessages.Add(outbox);
    }
}

using Hiredaily.Modules.Jobs.Application.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Hiredaily.Modules.Jobs.Infra.Persistence.SQL.Outbox;

public static class OutboxEntityMapping
{
    public static void ConfigureOutbox(this ModelBuilder modelBuilder)
    {
        var outboxMessage = modelBuilder.Entity<OutboxMessage>();

        outboxMessage.ToTable("OutboxMessages");

        outboxMessage.HasKey(message => message.Id);

        outboxMessage.HasIndex(message => message.PublishedAt);
    }
}

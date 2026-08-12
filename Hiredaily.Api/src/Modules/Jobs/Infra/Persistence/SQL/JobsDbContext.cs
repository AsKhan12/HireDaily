using Hiredaily.Modules.Jobs.Application.Outbox;
using Hiredaily.Modules.Jobs.Domain;
using Hiredaily.Modules.Jobs.Infra.Persistence.SQL.Jobs;
using Hiredaily.Modules.Jobs.Infra.Persistence.SQL.Outbox;
using Microsoft.EntityFrameworkCore;

namespace Hiredaily.Modules.Jobs.Infra.Persistence.SQL;

public sealed class JobsDbContext(DbContextOptions<JobsDbContext> options) : DbContext(options)
{
    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ConfigureJob();
        modelBuilder.ConfigureOutbox();
    }
}

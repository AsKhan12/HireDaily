using Hiredaily.BuildingBlock.Application.Events;
using Hiredaily.BuildingBlock.Domain.Entity;
using Hiredaily.Modules.Jobs.Domain.Abstraction;

namespace Hiredaily.Modules.Jobs.Infra.Persistence.SQL;

public class UnitOfWork(
    JobsDbContext dbContext,
    IntegrationEventDispatcher dispatcher
    ) : IUnitOfWork
{
    public async Task CommitAsync(CancellationToken ct = default)
    {
        var entities = dbContext.ChangeTracker
            .Entries<IEntity>()
            .Select(e => e.Entity)
            .ToList();

        var events = entities
            .SelectMany(e => e.Events)
            .ToList();

        await using var transaction =
            await dbContext.Database.BeginTransactionAsync(ct);

        try
        {
            await dbContext.SaveChangesAsync(ct);

            foreach (var domainEvent in events)
            {
                await dispatcher.Dispatch(domainEvent);
            }

            if (dbContext.ChangeTracker.HasChanges())
            {
                await dbContext.SaveChangesAsync(ct);
            }

            foreach (var entity in entities)
            {
                entity.ClearEvents();
            }

            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}
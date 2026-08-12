using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.Modules.Jobs.Domain;
using Hiredaily.Modules.Jobs.Domain.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace Hiredaily.Modules.Jobs.Infra.Persistence.SQL.Jobs;

public class JobRepository(JobsDbContext dbContext) : IJobRepository
{
    public async Task AddAsync(Job job, CancellationToken cancellationToken = default)
    {
        await dbContext.Jobs.AddAsync(job, cancellationToken);
    }

    public async Task<bool> ExistsByIdAsync(JobId id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Jobs.AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Job?> GetByIdAsync(JobId id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Jobs.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
    public async Task<List<Job>> GetByOrganizationIdAsync(OrganizationId id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Jobs.Where(x => x.OrganizationId == id).ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

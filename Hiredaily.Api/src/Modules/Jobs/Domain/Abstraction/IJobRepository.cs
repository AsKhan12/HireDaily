using Hiredaily.BuildingBlock.Domain.EntityIds;

namespace Hiredaily.Modules.Jobs.Domain.Abstraction;

public interface IJobRepository
{
    Task<Job?> GetByIdAsync(
        JobId id,
        CancellationToken cancellationToken = default);
    
    Task<List<Job>> GetByOrganizationIdAsync(
        OrganizationId id, CancellationToken cancellationToken = default);

    Task<bool> ExistsByIdAsync(
        JobId id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Job job,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
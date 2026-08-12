using Hiredaily.BuildingBlock.Domain.EntityIds;

namespace Hiredaily.Modules.Identity.Domain.Organization.Abstraction;

public interface IOrganizationRepository
{
    Task<Organization?> GetByIdAsync(
        OrganizationId id,
        CancellationToken cancellationToken = default);

    Task<Organization?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByIdAsync(
        OrganizationId id,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(
        string name,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        Organization organization,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

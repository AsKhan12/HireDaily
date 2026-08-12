using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.Modules.Identity.Domain.Organization;
using Hiredaily.Modules.Identity.Domain.Organization.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace Hiredaily.Modules.Identity.Infra.Persistence.Repostories;

public class OrganizationRepository(IdentityDbContext dbContext) : IOrganizationRepository
{
    public async Task AddAsync(Organization organization, CancellationToken cancellationToken = default)
    {
        await dbContext.Organizations.AddAsync(organization, cancellationToken);
    }

    public async Task<bool> ExistsByIdAsync(OrganizationId id, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(dbContext.Organizations.Any(x => x.Id == id));
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(dbContext.Organizations.Any(x => x.Name == name));
    }

    public async Task<Organization?> GetByIdAsync(OrganizationId id, CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(dbContext.Organizations.FirstOrDefault(x => x.Id == id));
    }

    public async Task<Organization?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await dbContext.Organizations.FirstOrDefaultAsync(
            x => x.Username == email,
            cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

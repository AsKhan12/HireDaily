using Hiredaily.Modules.Identity.Application.Abstraction;
using Hiredaily.Modules.Identity.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Hiredaily.Modules.Identity.Infra.Persistence.Repostories;

public class RefreshTokenStoreRepository(IdentityDbContext dbContext) : IRefreshTokenStoreRepository
{
    public async Task<RefreshTokenStore?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await dbContext.RefreshTokenStores.FirstOrDefaultAsync(x => x.Token == token, cancellationToken);
    }

    public async Task<RefreshTokenStore?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.RefreshTokenStores.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<RefreshTokenStore?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await dbContext.RefreshTokenStores.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }

    public async Task<RefreshTokenStore?> GetByOrgIdAsync(Guid organizationId, CancellationToken cancellationToken = default)
    {
        return await dbContext.RefreshTokenStores.FirstOrDefaultAsync(x => x.OrganizationId == organizationId, cancellationToken);
    }

    public async Task AddAsync(RefreshTokenStore store, CancellationToken cancellationToken = default)
    {
        await dbContext.RefreshTokenStores.AddAsync(store, cancellationToken);
    }

    public async Task UpdateAsync(RefreshTokenStore store, CancellationToken cancellationToken = default)
    {
        dbContext.RefreshTokenStores.Update(store);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var store = await GetByIdAsync(id, cancellationToken);
        if (store is not null)
        {
            dbContext.RefreshTokenStores.Remove(store);
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

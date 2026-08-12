using Hiredaily.Modules.Identity.Application.Models;

namespace Hiredaily.Modules.Identity.Application.Abstraction;

public interface IRefreshTokenStoreRepository
{
    Task<RefreshTokenStore?> GetByTokenAsync(string token, CancellationToken cancellationToken = default);
    Task<RefreshTokenStore?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(RefreshTokenStore store, CancellationToken cancellationToken = default);
    Task UpdateAsync(RefreshTokenStore store, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<RefreshTokenStore?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<RefreshTokenStore?> GetByOrgIdAsync(Guid organizationId, CancellationToken cancellationToken = default);
}

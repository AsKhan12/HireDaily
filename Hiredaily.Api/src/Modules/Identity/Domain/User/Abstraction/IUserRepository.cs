using Hiredaily.BuildingBlock.Domain.EntityIds;

namespace Hiredaily.Modules.Identity.Domain.User.Abstraction;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(
        UserId id,
        CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsByIdAsync(
        UserId id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        User user,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

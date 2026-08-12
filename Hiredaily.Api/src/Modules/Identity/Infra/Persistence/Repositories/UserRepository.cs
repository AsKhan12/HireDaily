using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.Modules.Identity.Domain.User;
using Hiredaily.Modules.Identity.Domain.User.Abstraction;
using Microsoft.EntityFrameworkCore;

namespace Hiredaily.Modules.Identity.Infra.Persistence.Repostories;

public class UserRepository(IdentityDbContext dbContext) : IUserRepository
{
    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await dbContext.Users.AddAsync(user, cancellationToken);
    }

    public async Task<bool> ExistsByIdAsync(UserId id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users.AnyAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await dbContext.Users.FirstOrDefaultAsync(x => x.Username == email, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

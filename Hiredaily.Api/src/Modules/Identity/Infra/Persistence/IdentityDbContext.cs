using Hiredaily.Modules.Identity.Domain.Organization;
using Hiredaily.Modules.Identity.Domain.User;
using Hiredaily.Modules.Identity.Application.Models;
using Microsoft.EntityFrameworkCore;
using Hiredaily.Modules.Identity.Infra.Persistence.EntityMappings;

namespace Hiredaily.Modules.Identity.Infra.Persistence;

public sealed class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshTokenStore> RefreshTokenStores => Set<RefreshTokenStore>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ConfigureOrganization();
        modelBuilder.ConfigureUser();
        modelBuilder.ConfigureRefreshTokenStore();
    }

    // ...existing code...
}

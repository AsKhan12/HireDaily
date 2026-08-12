using Hiredaily.Modules.Identity.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Hiredaily.Modules.Identity.Infra.Persistence.EntityMappings;

public static class RefreshTokenStoreEntityMapping
{
    public static void ConfigureRefreshTokenStore(this ModelBuilder modelBuilder)
    {
        var refreshTokenStore = modelBuilder.Entity<RefreshTokenStore>();

        refreshTokenStore.HasKey(x => x.Id);
        refreshTokenStore.ToTable("RefreshTokenStores");

        refreshTokenStore.Property(x => x.Id)
            .HasColumnName("Id")
            .ValueGeneratedNever();

        refreshTokenStore.Property(x => x.UserId)
            .HasColumnName("UserId")
            .IsRequired(false);

        refreshTokenStore.Property(x => x.OrganizationId)
            .HasColumnName("OrganizationId")
            .IsRequired(false);

        refreshTokenStore.Property(x => x.Token)
            .HasColumnName("Token")
            .HasMaxLength(512)
            .IsRequired();

        refreshTokenStore.Property(x => x.ExpiresAt)
            .HasColumnName("ExpiresAt")
            .IsRequired();

        refreshTokenStore.Property(x => x.CreatedAt)
            .HasColumnName("CreatedAt")
            .IsRequired();

        // Create index on Token for fast lookups
        refreshTokenStore.HasIndex(x => x.Token)
            .IsUnique();

        // Create index on ExpiresAt for cleanup operations
        refreshTokenStore.HasIndex(x => x.ExpiresAt);
    }
}

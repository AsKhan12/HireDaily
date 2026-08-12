using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.Modules.Identity.Domain.User;
using Hiredaily.Modules.Identity.Domain.User.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hiredaily.Modules.Identity.Infra.Persistence.EntityMappings;

public static class UserEntityMapping
{
    public static void ConfigureUser(this ModelBuilder modelBuilder)
    {
        var user = modelBuilder.Entity<User>();

        user.ToTable("Users");

        user.HasKey(x => x.Id);

        user.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new UserId(value))
            .ValueGeneratedNever();

        user.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        user.HasIndex(x => x.Username)
            .IsUnique();

        user.Property(x => x.CreatedAt)
            .IsRequired();

        user.Property(x => x.UpdatedAt);

        user.Ignore(x => x.Events);

        user.OwnsOne(x => x.Address, (OwnedNavigationBuilder<User, UserAddress> address) =>
        {
            address.Property(x => x.IsInitialized)
                .HasColumnName("AddressInitialized")
                .HasDefaultValue(true)
                .IsRequired();

            address.OwnsOne(x => x.Locatoin, location =>
            {
                location.Property(x => x.Lat)
                    .HasColumnName("Latitude")
                    .HasMaxLength(32)
                    .IsRequired(false);

                location.Property(x => x.Long)
                    .HasColumnName("Longitude")
                    .HasMaxLength(32)
                    .IsRequired(false);
            });

            address.OwnsOne(x => x.PostalAddress, postalAddress =>
            {
                postalAddress.Property(x => x.AddressLine1)
                    .HasColumnName("AddressLine1")
                    .HasMaxLength(300)
                    .IsRequired(false);

                postalAddress.Property(x => x.AddressLine2)
                    .HasColumnName("AddressLine2")
                    .HasMaxLength(300);

                postalAddress.Property(x => x.City)
                    .HasColumnName("City")
                    .HasMaxLength(120)
                    .IsRequired(false);

                postalAddress.Property(x => x.State)
                    .HasColumnName("State")
                    .HasMaxLength(120)
                    .IsRequired(false);

                postalAddress.Property(x => x.Country)
                    .HasColumnName("Country")
                    .HasMaxLength(120)
                    .IsRequired(false);

                postalAddress.Property(x => x.PostalCode)
                    .HasColumnName("PostalCode")
                    .HasMaxLength(32)
                    .IsRequired(false);
            });

            address.OwnsOne(x => x.ContactDetails, contactDetails =>
            {
                contactDetails.Property(x => x.Email)
                    .HasColumnName("Email")
                    .HasMaxLength(320)
                    .IsRequired(false);

                contactDetails.Property(x => x.Phone)
                    .HasColumnName("Phone")
                    .HasMaxLength(32)
                    .IsRequired(false);
            });

            address.Navigation(x => x.Locatoin).IsRequired();
            address.Navigation(x => x.PostalAddress).IsRequired();
            address.Navigation(x => x.ContactDetails).IsRequired();
        });

        user.Property(x => x.PasswordHash)
            .HasColumnName("PasswordHash")
            .HasMaxLength(256)
            .IsRequired();

        user.OwnsMany(x => x.Skills, skill =>
        {
            skill.ToTable("UserSkills");
            skill.WithOwner().HasForeignKey("UserId");
            skill.Property<int>("Id");
            skill.HasKey("Id");

            skill.Property(x => x.Name)
                .HasMaxLength(120)
                .IsRequired(false);

            skill.Property(x => x.Field)
                .HasMaxLength(120)
                .IsRequired(false);

            skill.Property(x => x.Description)
                .HasMaxLength(1000)
                .IsRequired(false);

            skill.Property(x => x.SkillLevel)
                .HasConversion<string>()
                .HasMaxLength(50);
        });

        user.Navigation(x => x.Address).IsRequired();
        user.Navigation(x => x.Skills).AutoInclude();
    }
}

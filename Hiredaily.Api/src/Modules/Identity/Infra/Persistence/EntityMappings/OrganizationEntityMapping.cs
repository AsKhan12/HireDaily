using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Domain.ValueObjects;
using Hiredaily.Modules.Identity.Domain.Organization;
using Hiredaily.Modules.Identity.Domain.Organization.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hiredaily.Modules.Identity.Infra.Persistence.EntityMappings;

public static class OrganizationEntityMapping
{
    public static void ConfigureOrganization(this ModelBuilder modelBuilder)
    {
        var organization = modelBuilder.Entity<Organization>();

        organization.ToTable("Organizations");

        organization.HasKey(x => x.Id);

        organization.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new OrganizationId(value))
            .ValueGeneratedNever();

        organization.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        organization.Property(x => x.Username)
            .HasColumnName("Email")
            .HasMaxLength(320)
            .IsRequired();

        organization.HasIndex(x => x.Username)
            .IsUnique();
        
        organization.HasIndex(x => x.Name)
            .IsUnique();

        organization.Property(x => x.Description)
            .HasMaxLength(1000)
            .IsRequired(false);

        organization.Property(x => x.PasswordHash)
            .HasColumnName("PasswordHash")
            .HasMaxLength(256)
            .IsRequired();
        organization.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        organization.Property(x => x.CreatedAt)
            .IsRequired();

        organization.Ignore(x => x.Events);

        organization.OwnsOne(x => x.Address, (OwnedNavigationBuilder<Organization, OrganizationAddress> address) =>
        {
            address.Property(x => x.IsInitialized)
                .HasColumnName("AddressInitialized")
                .HasDefaultValue(true)
                .IsRequired();

            address.OwnsOne(x => x.Location, (OwnedNavigationBuilder<OrganizationAddress, GeoLocation> location) =>
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

            address.OwnsOne(x => x.PostalAddress, (OwnedNavigationBuilder<OrganizationAddress, PostalAddress> postalAddress) =>
            {
                postalAddress.Property(x => x.AddressLine1)
                    .HasColumnName("AddressLine1")
                    .HasMaxLength(300)
                    .IsRequired(false);

                postalAddress.Property(x => x.AddressLine2)
                    .HasColumnName("AddressLine2")
                    .HasMaxLength(300)
                    .IsRequired(false);

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

            address.OwnsOne(x => x.ContactDetails, (OwnedNavigationBuilder<OrganizationAddress, OrganizationContactDetails> contactDetails) =>
            {
                contactDetails.Property(x => x.Email)
                    .HasColumnName("ContactEmail")
                    .HasMaxLength(320)
                    .IsRequired(false);

                contactDetails.Property(x => x.Phone)
                    .HasColumnName("Phone")
                    .HasMaxLength(32)
                    .IsRequired(false);

                contactDetails.Property(x => x.WebsiteUrl)
                    .HasColumnName("WebsiteUrl")
                    .HasMaxLength(500)
                    .IsRequired(false);
            });

            address.Navigation(x => x.Location).IsRequired();
            address.Navigation(x => x.PostalAddress).IsRequired();
            address.Navigation(x => x.ContactDetails).IsRequired();
        });

        organization.Navigation(x => x.Address)
            .IsRequired();
    }
}

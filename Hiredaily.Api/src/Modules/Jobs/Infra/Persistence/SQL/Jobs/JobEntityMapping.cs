using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Domain.ValueObjects;
using Hiredaily.Modules.Jobs.Domain;
using Hiredaily.Modules.Jobs.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hiredaily.Modules.Jobs.Infra.Persistence.SQL.Jobs;

public static class JobEntityMapping
{
    public static void ConfigureJob(this ModelBuilder modelBuilder)
    {
        var job = modelBuilder.Entity<Job>();

        job.ToTable("Jobs");

        job.HasKey(x => x.Id);

        job.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new JobId(value))
            .ValueGeneratedNever();

        job.Property(x => x.OrganizationId)
            .HasConversion(
                id => id.Value,
                value => new OrganizationId(value))
            .IsRequired();

        job.Property(x => x.CreatedAt)
            .IsRequired();

        job.Property(x => x.UpdatedAt);

        job.Ignore(x => x.Events);

        job.OwnsOne(x => x.HourlyRate, (OwnedNavigationBuilder<Job, Money> hourlyRate) =>
        {
            hourlyRate.Property(x => x.Amount)
                .HasColumnName("HourlyRateAmount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            hourlyRate.Property(x => x.Currency)
                .HasColumnName("HourlyRateCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        job.OwnsOne(x => x.JobSite, (OwnedNavigationBuilder<Job, JobSite> jobSite) =>
        {
            jobSite.OwnsOne(x => x.Location, location =>
            {
                location.Property(x => x.Lat)
                    .HasColumnName("Latitude")
                    .HasMaxLength(32)
                    .IsRequired();

                location.Property(x => x.Long)
                    .HasColumnName("Longitude")
                    .HasMaxLength(32)
                    .IsRequired();
            });

            jobSite.OwnsOne(x => x.Address, address =>
            {
                address.Property(x => x.AddressLine1)
                    .HasColumnName("AddressLine1")
                    .HasMaxLength(300)
                    .IsRequired();

                address.Property(x => x.AddressLine2)
                    .HasColumnName("AddressLine2")
                    .HasMaxLength(300);

                address.Property(x => x.City)
                    .HasColumnName("City")
                    .HasMaxLength(120)
                    .IsRequired();

                address.Property(x => x.State)
                    .HasColumnName("State")
                    .HasMaxLength(120)
                    .IsRequired();

                address.Property(x => x.Country)
                    .HasColumnName("Country")
                    .HasMaxLength(120)
                    .IsRequired();

                address.Property(x => x.PostalCode)
                    .HasColumnName("PostalCode")
                    .HasMaxLength(32)
                    .IsRequired();
            });
        });

        job.OwnsMany(x => x.RequiredSkills, skill =>
        {
            skill.ToTable("JobRequiredSkills");
            skill.WithOwner().HasForeignKey("JobId");
            skill.Property<int>("Id");
            skill.HasKey("Id");

            skill.Property(x => x.Name)
                .HasMaxLength(120)
                .IsRequired();

            skill.Property(x => x.Field)
                .HasMaxLength(120)
                .IsRequired();

            skill.Property(x => x.Description)
                .HasMaxLength(1000)
                .IsRequired();

            skill.Property(x => x.SkillLevel)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();
        });

        job.Navigation(x => x.HourlyRate).IsRequired();
        job.Navigation(x => x.JobSite).IsRequired();
        job.Navigation(x => x.RequiredSkills).AutoInclude();
    }
}

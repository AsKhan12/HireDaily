using Hiredaily.BuildingBlock.Application.Mediator.Handlers;
using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;
using Hiredaily.Modules.Jobs.API.Chassis;
using Hiredaily.Modules.Jobs.Application.CreateJob;
using Hiredaily.Modules.Jobs.Application.GetJob;
using Hiredaily.Modules.Jobs.Application.GetOrganizationJob;
using Hiredaily.Modules.Jobs.Application.Outbox;
using Hiredaily.Modules.Jobs.Application.UpdateHourlyRate;
using Hiredaily.Modules.Jobs.Application.UpdateJobSite;
using Hiredaily.Modules.Jobs.Application.UpdateRequiredSkills;
using Hiredaily.Modules.Jobs.Domain.Abstraction;
using Hiredaily.Modules.Jobs.Infra.Persistence.SQL;
using Hiredaily.Modules.Jobs.Infra.Persistence.SQL.Jobs;
using Hiredaily.Modules.Jobs.Infra.Persistence.SQL.Outbox;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hiredaily.BuildingBlock.API;
using Hiredaily.BuildingBlock.Application.Events;
using Hiredaily.Modules.Jobs.Domain.Events;
using Hiredaily.Modules.Jobs.Application.UpdateJobTitle;

namespace Hiredaily.Modules.Jobs.API;

public static class ServiceRegistration
{
    public static void AddJobServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatr<BehaviorConfiguration>();
        services.AddCommonServices();
        services.AddSingleton<ValidationPipelineBehavior>();

        services.AddScoped<ICommandHandler<CreateJobCommand>, CreateJobCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateJobHourlyRateCommand>, UpdateJobHourlyRateCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateJobSiteCommand>, UpdateJobSiteCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateJobRequiredSkillsCommand>, UpdateJobRequiredSkillsCommandHandler>();
        services.AddScoped<ICommandHandler<UpdateJobTitleCommand>, UpdateJobTitleCommandHandler>();
        services.AddScoped<IRequestHandler<GetJobRequest, GetJobResponse>, GetJobRequestHandler>();
        services.AddScoped<IRequestHandler<GetOrganizatoinJobRequest, GetOrganizationJobResponse>, GetOrganizationJobRequestHandler>();

        services.AddScoped<IValidator<CreateJobCommand>, CreateJobCommandValidator>();
        services.AddScoped<IValidator<UpdateJobTitleCommand>, UpdateJobTitleCommandValidator>();
        services.AddScoped<IValidator<UpdateJobHourlyRateCommand>, UpdateJobHourlyRateCommandValidator>();
        services.AddScoped<IValidator<UpdateJobSiteCommand>, UpdateJobSiteCommandValidator>();
        services.AddScoped<IValidator<UpdateJobRequiredSkillsCommand>, UpdateJobRequiredSkillsCommandValidator>();
        services.AddScoped<IValidator<GetJobRequest>, GetJobRequestValidator>();
        services.AddScoped<IValidator<GetOrganizatoinJobRequest>, GetOrganizationJobRequestValidator>();

        services.AddScoped<IDomainEventHandler<JobCreatedEvent>, JobCreatedEventHandler>();
        services.AddScoped<IDomainEventHandler<JobTitleUpdatedEvent>, JobTitleUpdatedEventHandler>();
        services.AddScoped<IDomainEventHandler<JobHourlyRateUpdatedEvent>, JobHourlyRateUpdatedEventEventHandler>();
        services.AddScoped<IDomainEventHandler<JobSiteUpdatedEvent>, JobSiteUpdatedEventHandler>();
        services.AddScoped<IDomainEventHandler<JobRequiredSkillsUpdatedEvent>, JobRequiredSkillsUpdatedEventHandler>();
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IJobRepository, JobRepository>();
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        // services.AddScoped<IJobFeedRepository, CosmosJobFeedRepository>();
        services.AddSingleton(_ =>
        {
            var connectionString = configuration.GetConnectionString("Cosmos")
                ?? configuration["Cosmos:ConnectionString"]
                ?? throw new InvalidOperationException("Cosmos connection string is required.");

            return new CosmosClient(connectionString);
        });
        services.AddSingleton(provider =>
        {
            var client = provider.GetRequiredService<CosmosClient>();
            var databaseName = configuration["Cosmos:DatabaseName"] ?? "Hiredaily";
            var containerName = configuration["Cosmos:JobFeedContainerName"] ?? "JobFeed";

            return client.GetContainer(databaseName, containerName);
        });
        services.AddDbContext<JobsDbContext>((sp, options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("JobsDbConnection"));
        });
    }

    public static IServiceCollection AddJobsMessagePublisherServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMessaging(configuration);            
        services.AddScoped<IOutboxRepository, OutboxRepository>();
        services.AddScoped<JobOutboxMessagePublisher>();
        services.AddDbContext<JobsDbContext>((sp, options) =>
        {
            options.UseSqlServer(configuration.GetConnectionString("JobsDbConnection"));
        });
        return services;
    }
}

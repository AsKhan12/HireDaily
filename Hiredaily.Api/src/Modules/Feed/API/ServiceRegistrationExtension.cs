using Hiredaily.Modules.Feed.Application;
using Hiredaily.Modules.Feed.Application.Common;
using Hiredaily.Modules.Feed.Application.Write;
using Hiredaily.Modules.Feed.Infra.Repository;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hiredaily.Modules.Feed.Infra.Settings;
using Microsoft.Extensions.Options;
using Azure.Identity;
using Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;
using Hiredaily.Modules.Feed.Application.Read;
using Hiredaily.BuildingBlock.Application.Mediator.Handlers;
using Hiredaily.BuildingBlock.API;
namespace Hiredaily.Modules.Feed.API;

public static class ServiceRegistrationExtension
{
    public static IServiceCollection AddFeedServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddKeyedScoped<IIntegrationEventHandler, JobCreatedMessageHandler>("job.created");
        services.AddKeyedScoped<IIntegrationEventHandler, JobHourlyRateUpdatedMessageHandler>("job.hourly-rate.updated");
        services.AddKeyedScoped<IIntegrationEventHandler, JobSiteUpdatedMessageHandler>("job.site.updated");
        services.AddKeyedScoped<IIntegrationEventHandler, JobRequiredSkillsUpdatedMessageHandler>("job.skills.updated");
        services.AddSingleton<IntegrationHandlerFactory>();
        services.AddScoped<IValidator<JobFeedRequest>, JobFeedRequestValidator>();
        services.AddScoped<IRequestHandler<JobFeedRequest, IEnumerable<JobFeedResponse>>, JobFeedRequestHandler>();
        services.AddMessaging(configuration);

        services.AddOptions<CosmosSettings>()
            .Bind(configuration.GetSection(CosmosSettings.SectionName))
            .Validate(option =>
            {
                if(
                    string.IsNullOrWhiteSpace(option.Endpoint)
                    || string.IsNullOrWhiteSpace(option.Container)
                    || string.IsNullOrWhiteSpace(option.Database))
                    return false;
                return true;
            })
            .ValidateOnStart();

        services.AddScoped<IJobFeedRepository, CosmosJobFeedRepository>();
        services.AddSingleton(sp =>
        {
            var cosmosSettings = sp.GetRequiredService<IOptions<CosmosSettings>>().Value;
            return new CosmosClient(
                cosmosSettings.Endpoint, 
                new DefaultAzureCredential(),
                new CosmosClientOptions
                {
                    SerializerOptions = new CosmosSerializationOptions
                    {
                        PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                    }
                });
        });
        services.AddSingleton(sp =>
        {
            var client = sp.GetRequiredService<CosmosClient>();            
            var cosmosSettings = sp.GetRequiredService<IOptions<CosmosSettings>>().Value;
            return client.GetContainer(cosmosSettings.Database, cosmosSettings.Container);
        });

        return services;
    }
}
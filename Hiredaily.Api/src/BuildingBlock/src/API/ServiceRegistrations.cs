using Hiredaily.BuildingBlock.Application.Events;
using Hiredaily.BuildingBlock.Application.Mediator;
using Hiredaily.BuildingBlock.Application.Mediator.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Azure.Messaging.ServiceBus;
using Hiredaily.BuildingBlock.Infra.Messaging.RabbitMq;
using Hiredaily.BuildingBlock.Infra.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Hiredaily.BuildingBlock.API;

public static class ServiceRegistrations
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services)
    {
        services.AddScoped<IntegrationEventDispatcher>();
        return services;
    }
    public static IServiceCollection AddMediatr<TConfiguration>(this IServiceCollection services)
        where TConfiguration : class, IBehaviorConfiguration
    {
        services.AddSingleton<IBehaviorCollection, BehaviorCollection>();
        services.AddSingleton<IBehaviorConfiguration, TConfiguration>();
        services.AddSingleton<PipelineStartup>();
        services.AddSingleton<IMediatr,Mediatr>();
        return services;
    }

    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration.GetValue<string>("MessagingProvider");
        _ =  provider switch
        {
            "RabbitMq" => AddRabbitMq(services,configuration),
            _ => AddServiceBus(services,configuration)
            
        };
        return services;

    }
    private static IServiceCollection AddRabbitMq(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IMessagePublisher, RabbitMqPublisher>();
        services.AddScoped<IMessageConsumer, RabbitMqConsumer>();
        services
            .AddOptions<RabbitmqSettings>()
            .Bind(configuration.GetSection(RabbitmqSettings.SectionName))
            .ValidateOnStart();

        services.AddSingleton<RabbitMqConnectionFactory>();

        services.AddSingleton<IConnection>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<RabbitmqSettings>>().Value;
        
            return sp.GetRequiredService<RabbitMqConnectionFactory>()
                .Create(options)
                .GetAwaiter()
                .GetResult();
        });

        services
            .AddSingleton<IChannel>(sp =>
                sp.GetRequiredService<IConnection>()
                    .CreateChannelAsync()
                    .GetAwaiter()
                    .GetResult());

        return services;
    }

    private static IServiceCollection AddServiceBus(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IMessagePublisher, ServiceBusPublisher>();
        services
            .AddOptions<ServiceBusSettings>()
            .Bind(configuration.GetSection(ServiceBusSettings.SectionName))
            .ValidateOnStart();

        services.AddSingleton(sp =>
        {
            var options = sp.GetRequiredService<IOptions<ServiceBusSettings>>().Value;
            return new ServiceBusClient(options.Connectionstring);
        });
        return services;
    }
}
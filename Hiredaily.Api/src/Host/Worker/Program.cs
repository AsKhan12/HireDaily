using Worker;
using Hiredaily.Modules.Feed.API;
using Hiredaily.Modules.Jobs.API;
var builder = Host.CreateApplicationBuilder(args);
builder.Services
        .AddFeedServices(builder.Configuration)
        .AddJobsMessagePublisherServices(builder.Configuration)
        .AddHostedService<JobEventsConsumerService>()
        .AddHostedService<JobEventPublisherSerice>();

var host = builder.Build();
host.Run();

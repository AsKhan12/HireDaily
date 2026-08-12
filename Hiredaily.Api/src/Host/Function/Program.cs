using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using Hiredaily.Modules.Feed.API;
using Hiredaily.Modules.Jobs.API;
using Microsoft.Extensions.Configuration;

var builder = FunctionsApplication.CreateBuilder(args);
builder.ConfigureFunctionsWebApplication();

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    // .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// builder.Services.AddOpenTelemetry()
//     .UseFunctionsWorkerDefaults()
//     .UseAzureMonitorExporter();
    
builder.Services
    .AddFeedServices(builder.Configuration)
    .AddJobsMessagePublisherServices(builder.Configuration);

builder.Build().Run();

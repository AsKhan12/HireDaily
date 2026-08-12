using Azure.Monitor.OpenTelemetry.AspNetCore;
using Hiredaily.Host.API.Middlewares;
using Hiredaily.Modules.Feed.API;
using Hiredaily.Modules.Identity.API;
using Hiredaily.Modules.Jobs.API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services
    .AddOpenTelemetry()
    .UseAzureMonitor();

builder.Services.AddEndpointsApiExplorer();

// Allow local frontend (Vite) during development
builder.Services.AddCors(options =>
{
    options.AddPolicy("LocalDev", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
});
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddJobServices(builder.Configuration);
builder.Services.AddFeedServices(builder.Configuration);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{// Enable CORS for local development frontend
    app.UseCors("LocalDev");
    app.MapOpenApi();

    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseExceptionHandler();
app.UseIdentityModule();
app.UseJobsModule();
app.UseFeedModule();

app.Run();

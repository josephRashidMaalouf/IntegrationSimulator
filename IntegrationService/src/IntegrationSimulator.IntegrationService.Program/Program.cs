using IntegrationSimulator.IntegrationService.Application.Services;
using IntegrationSimulator.IntegrationService.Domain.Interfaces;
using IntegrationSimulator.IntegrationService.Infrastructure.HttpClients;
using IntegrationSimulator.IntegrationService.Infrastructure.Persistence;
using IntegrationSimulator.IntegrationService.Infrastructure.Persistence.Repositories;
using IntegrationSimulator.IntegrationService.Infrastructure.PollyHandlers;
using IntegrationSimulator.IntegrationService.Program.HostedServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient<IJobAdClient, PlatsbankenClient>()
    .SetHandlerLifetime(TimeSpan.FromMinutes(5))
    .AddPolicyHandler(RetryPolicy.Get());


builder.Services.AddHttpClient<IFakeERPClient, FakeERPClient>()
    .SetHandlerLifetime(TimeSpan.FromMinutes(5))
    .AddPolicyHandler(RetryPolicy.Get());

builder.Services.AddDbContext<JobAdsMetaDataContext>();


builder.Services.AddSerilog(config =>
{
    config
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day);
});


builder.Services.AddScoped<IMetaDataRepository, MetaDataRepository>();
builder.Services.AddScoped<IPollingCoordinator, PollingCoordinator>();

builder.Services.AddHostedService<PollingService>();

var app  = builder.Build();

await ApplyMigrations();

//Make sure the database have been set up before continuing
Thread.Sleep(10000);

await app.RunAsync();


async Task ApplyMigrations()
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<JobAdsMetaDataContext>();

    if ((await dbContext.Database.GetPendingMigrationsAsync()).Any())
    {
        await dbContext.Database.MigrateAsync();
    }
}
// See https://aka.ms/new-console-template for more information

using IntegrationSimulator.IntegrationService.Application.Services;
using IntegrationSimulator.IntegrationService.Domain.Interfaces;
using IntegrationSimulator.IntegrationService.Infrastructure.HttpClients;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;


var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient<IJobAdClient, PlatsbankenClient>();
builder.Services.AddHttpClient<IFakeERPClient, FakeERPClient>();


builder.Services.AddSerilog(config =>
{
    config
        .MinimumLevel.Information()
        .WriteTo.Console()
        .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day);
});



builder.Services.AddScoped<IPollingCoordinator, PollingCoordinator>();

var app  = builder.Build();

var coordinator = app.Services.GetRequiredService<IPollingCoordinator>();

await coordinator.Execute();
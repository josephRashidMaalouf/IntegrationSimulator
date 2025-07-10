using IntegrationSimulator.IntegrationService.Domain.Interfaces;
using Microsoft.Extensions.Hosting;

namespace IntegrationSimulator.IntegrationService.Program.HostedServices;

public class PollingService : BackgroundService
{
    private readonly IPollingCoordinator _coordinator;

    public PollingService(IPollingCoordinator coordinator)
    {
        _coordinator = coordinator;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var trace = Guid.NewGuid();
            await _coordinator.Execute(trace);

            try
            {
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }

        }
    }
}
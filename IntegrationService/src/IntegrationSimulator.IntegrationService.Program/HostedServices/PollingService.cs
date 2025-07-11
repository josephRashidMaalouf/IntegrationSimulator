using IntegrationSimulator.IntegrationService.Domain.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IntegrationSimulator.IntegrationService.Program.HostedServices;

public class PollingService : BackgroundService
{
    private readonly IPollingCoordinator _coordinator;
    private readonly ILogger<PollingService> _logger;

    public PollingService(IPollingCoordinator coordinator, ILogger<PollingService> logger)
    {
        _coordinator = coordinator;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var trace = Guid.NewGuid();

            _logger.LogInformation("");
            await _coordinator.ExecuteAsync(trace);

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
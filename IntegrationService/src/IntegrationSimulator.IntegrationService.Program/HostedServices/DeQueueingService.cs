using IntegrationSimulator.IntegrationService.Infrastructure.MessageQueueing;
using Microsoft.Extensions.Hosting;

namespace IntegrationSimulator.IntegrationService.Program.HostedServices;

public class DeQueueingService : BackgroundService
{
    private readonly Consumer _consumer;

    public DeQueueingService(Consumer consumer)
    {
        _consumer = consumer;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        bool open = false;

        while (!stoppingToken.IsCancellationRequested)
        {
            if (!open)
            {
                await _consumer.OpenQueueAsync(stoppingToken);
                open = true;
            }
        }
    }
}
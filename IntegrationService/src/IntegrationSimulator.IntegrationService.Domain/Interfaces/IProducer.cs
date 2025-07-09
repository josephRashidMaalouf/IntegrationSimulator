using IntegrationSimulator.IntegrationService.Domain.Models;

namespace IntegrationSimulator.IntegrationService.Domain.Interfaces;

public interface IProducer
{
    Task PublishToQueueAsync(QueueAdsDto ads);
}
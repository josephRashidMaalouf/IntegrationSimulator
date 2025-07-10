using System.Text;
using System.Text.Json;
using IntegrationSimulator.IntegrationService.Domain.Interfaces;
using IntegrationSimulator.IntegrationService.Domain.Models;
using RabbitMQ.Client;

namespace IntegrationSimulator.IntegrationService.Infrastructure.MessageQueueing;

public class Producer : IProducer
{
    private readonly RabbitMQService _mqService;
    private readonly IRabbitMQConfiguration _config;
     

    public Producer(RabbitMQService mqService)
    {
        _mqService = mqService;
    }

    public async Task PublishToQueueAsync(QueueAdsDto ads)
    {
        var channelConnection = await _mqService.DeclareDurableQueueAsync();
        
        var jsonAds = JsonSerializer.Serialize(ads);
        var body = Encoding.UTF8.GetBytes(jsonAds);

        await channelConnection.channel.BasicPublishAsync(
            exchange: _config.ExchangeName,
            routingKey: _config.RoutingKey,
            body: body
        );

        await channelConnection.channel.CloseAsync();
        await channelConnection.connection.CloseAsync();

    }
}
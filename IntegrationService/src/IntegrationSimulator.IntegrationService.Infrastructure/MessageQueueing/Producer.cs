using System.Text;
using System.Text.Json;
using IntegrationSimulator.IntegrationService.Domain.Interfaces;
using IntegrationSimulator.IntegrationService.Domain.Models;
using RabbitMQ.Client;

namespace IntegrationSimulator.IntegrationService.Infrastructure.MessageQueueing;

public class Producer : IProducer
{
    private readonly IRabbitMQConfiguration _config;
    private readonly IConnectionFactory _factory;

    public Producer(IConnectionFactory factory, IRabbitMQConfiguration config)
    {
        _factory = factory;
        _config = config;
    }

    public async Task PublishToQueueAsync(QueueAdsDto ads)
    {
        await using var connection = await _factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(
            exchange: _config.ExchangeName,
            type: ExchangeType.Fanout,
            durable: true,
            autoDelete: false);

        var jsonAds = JsonSerializer.Serialize(ads);
        var body = Encoding.UTF8.GetBytes(jsonAds);

        var props = new BasicProperties()
        {
            Persistent = true
        };

        await channel.BasicPublishAsync(
            exchange: _config.ExchangeName,
            routingKey: "",
            mandatory: true,
            basicProperties: props,
            body: body
        );
    }
}
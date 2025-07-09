using System.Text;
using System.Text.Json;
using IntegrationSimulator.IntegrationService.Domain.Interfaces;
using IntegrationSimulator.IntegrationService.Domain.Models;
using IntegrationSimulator.IntegrationService.Domain.Models.Results;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace IntegrationSimulator.IntegrationService.Infrastructure.MessageQueueing;

public class Producer : IProducer
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly string _hostName;
    private readonly string _queueName;
    private readonly string _routingKey; 

    public Producer(IConnectionFactory connectionFactory, IConfiguration config)
    {
        _connectionFactory = connectionFactory;

        var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "";

        var rabbitMQConfig = config.GetSection("RabbitMQ");

        _routingKey = rabbitMQConfig["RoutingKey"] ?? "";
        _queueName = rabbitMQConfig["QueueName"] ?? "";
        _hostName = rabbitMQConfig["HostNameLocal"] ?? "";
        if (env == "docker")
        {
            _hostName = rabbitMQConfig["HostNameDocker"] ?? "";
        }
    }

    public async Task PublishToQueueAsync(QueueAdsDto ads)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _hostName
        };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: _queueName, 
            durable: false, 
            exclusive: false, 
            autoDelete: false,
            arguments: null);

        var jsonAds = JsonSerializer.Serialize(ads);
        var body = Encoding.UTF8.GetBytes(jsonAds);

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: _routingKey,
            body: body
        );

        await channel.CloseAsync();
        await connection.CloseAsync();

    }
}
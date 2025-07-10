using IntegrationSimulator.IntegrationService.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace IntegrationSimulator.IntegrationService.Infrastructure.MessageQueueing;

public class RabbitMQService
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly IRabbitMQConfiguration _config;

    public RabbitMQService(IConnectionFactory connectionFactory, IRabbitMQConfiguration config)
    {
        _connectionFactory = connectionFactory;
        _config = config;
    }

    public async Task<(IConnection connection, IChannel channel)> DeclareDurableQueueAsync()
    {
        await using var connection = await _connectionFactory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: _config.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        return (connection, channel);
    }
}
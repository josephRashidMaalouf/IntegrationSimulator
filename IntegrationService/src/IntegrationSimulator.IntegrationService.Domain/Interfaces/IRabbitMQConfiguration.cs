namespace IntegrationSimulator.IntegrationService.Domain.Interfaces;

public interface IRabbitMQConfiguration
{
    string HostName { get; }
    string QueueName { get; }
    string ExchangeName { get; }
    string ClientProvidedName { get; }
    string RoutingKey { get; }
    string Username { get; }
    string Password { get; }
    string Uri { get; }
}
using IntegrationSimulator.IntegrationService.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace IntegrationSimulator.IntegrationService.Program.ConfigurationModels;

public class RabbitMQConfiguration : IRabbitMQConfiguration
{
    public string HostName { get; }
    public string QueueName { get; }
    public string ExchangeName { get; }
    public string ClientProvidedName { get; }
    public string RoutingKey { get; }
    public string Username { get; }
    public string Password { get; }
    public string Uri { get; }

    public RabbitMQConfiguration(IConfiguration config)
    {
        var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "local";

        var rabbitMQConfig = config.GetSection("RabbitMQ");

        HostName = rabbitMQConfig["HostNameLocal"] ?? throw new InvalidOperationException("RabbitMQ HostNameLocal is not configured");
        QueueName = rabbitMQConfig["QueueName"] ?? throw new InvalidOperationException("RabbitMQ QueueName is not configured"); 
        ExchangeName = rabbitMQConfig["ExchangeName"] ?? throw new InvalidOperationException("RabbitMQ ExchangeName is not configured"); 
        ClientProvidedName = rabbitMQConfig["ClientProvidedName"] ?? throw new InvalidOperationException("RabbitMQ ClientProvidedName is not configured"); 
        RoutingKey = rabbitMQConfig["RoutingKey"] ?? throw new InvalidOperationException("RabbitMQ RoutingKey is not configured"); 
        Username = rabbitMQConfig["Username"] ?? throw new InvalidOperationException("RabbitMQ Username is not configured"); 
        Password = rabbitMQConfig["Password"] ?? throw new InvalidOperationException("RabbitMQ Password is not configured"); 
        Uri = rabbitMQConfig["UriLocal"] ?? throw new InvalidOperationException("RabbitMQ UriLocal is not configured"); 
        if (env == "docker")
        {
            HostName = rabbitMQConfig["HostNameDocker"] ?? throw new InvalidOperationException("RabbitMQ HostNameDocker is not configured"); 
            Uri = rabbitMQConfig["UriDocker"] ?? throw new InvalidOperationException("RabbitMQ UriDocker is not configured"); 
        }
    }
}
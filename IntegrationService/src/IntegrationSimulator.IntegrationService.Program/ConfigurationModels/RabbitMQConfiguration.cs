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
        //TODO: Implement an exception to throw if these configs are not set. The app should not be able to continue without them.
        var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "";

        var rabbitMQConfig = config.GetSection("RabbitMQ");

        HostName = rabbitMQConfig["HostNameLocal"] ?? "";
        if (env == "docker")
        {
            HostName = rabbitMQConfig["HostNameDocker"] ?? "";
        }

        QueueName = rabbitMQConfig["QueueName"] ?? "";
        ExchangeName = rabbitMQConfig["ExchangeName"] ?? "";
        ClientProvidedName = rabbitMQConfig["ClientProvidedName"] ?? "";
        RoutingKey = rabbitMQConfig["RoutingKey"] ?? "";
        Username = rabbitMQConfig["Username"] ?? "";
        Password = rabbitMQConfig["Password"] ?? "";
        Uri = rabbitMQConfig["Uri"] ?? "";
    }
}
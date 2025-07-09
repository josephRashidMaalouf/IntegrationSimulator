using IntegrationSimulator.IntegrationService.Domain.Interfaces;
using IntegrationSimulator.IntegrationService.Domain.Models;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using IntegrationSimulator.IntegrationService.Domain.Models.Results;
using IntegrationSimulator.IntegrationService.Infrastructure.Persistence.Repositories;
using System.Diagnostics;

namespace IntegrationSimulator.IntegrationService.Infrastructure.MessageQueueing;

public class Consumer
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly IFakeERPClient _erpClient;
    private readonly ILogger<Consumer> _logger;
    private readonly string _hostName;
    private readonly string _queueName;

    public Consumer(IConnectionFactory connectionFactory, IConfiguration config, IFakeERPClient erpClient, ILogger<Consumer> logger)
    {
        _connectionFactory = connectionFactory;
        _erpClient = erpClient;
        _logger = logger;

        var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "";

        var rabbitMQConfig = config.GetSection("RabbitMQ");

        _queueName = rabbitMQConfig["QueueName"] ?? "";
        _hostName = rabbitMQConfig["HostNameLocal"] ?? "";
        if (env == "docker")
        {
            _hostName = rabbitMQConfig["HostNameDocker"] ?? "";
        }
    }

    public async Task OpenQueueAsync(CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory()
        {
            HostName = _hostName
        };

        await using var connection = await factory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync(cancellationToken: cancellationToken);

        bool erpSuccess = false;
        ulong deliveryTag = 0;

        await channel.QueueDeclareAsync(
            queue: _queueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null, 
            cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var jsonString = Encoding.UTF8.GetString(body);
            deliveryTag = eventArgs.DeliveryTag;

            //TODO: Create an exception for this case
            var ads = JsonSerializer.Deserialize<QueueAdsDto>(jsonString) ?? throw new Exception();

            _logger.LogInformation($"Trace: {ads.Trace}. Received {ads.AdsData.NumberOfAds} ads from queue. Processing...");

            if (ads.AdsData.NumberOfAds != 0)
            {
                var dto = ads.AdsData.Ads
                    .Select(x => new PostNewJobToFakeERP(x.Id, x.Title, x.WorkplaceName, x.PublishedDate))
                    .ToList();
                var resultErp = await _erpClient.PostNewJobListingsAsync(dto, ads.Trace);

                if (resultErp is ErrorResult<List<PostNewJobToFakeERP>> erpErrorResult)
                {

                    foreach (var er in erpErrorResult.Errors)
                    {
                        _logger.LogWarning("Trace: {id}. Could not send ads to fake ERP: {reason}", er.Trace, er.Message);
                    }
                    return;
                }

            }
            erpSuccess = true;
            _logger.LogInformation("Trace: {id}. AdsData sent to fakeERP: {numOfAds}", ads.Trace, ads.AdsData.NumberOfAds);
        };

        if (!erpSuccess)
        {
            await channel.BasicRejectAsync(
                deliveryTag: deliveryTag,
                requeue: false,
                cancellationToken: cancellationToken);
        }
        else
        {
            await channel.BasicAckAsync(
                deliveryTag: deliveryTag,
                multiple: false,
                cancellationToken: cancellationToken);
        }

    }
}
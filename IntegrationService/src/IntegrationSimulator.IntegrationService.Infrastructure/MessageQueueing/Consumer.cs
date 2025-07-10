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
    private readonly IFakeERPClient _erpClient;
    private readonly ILogger<Consumer> _logger;
    private readonly IRabbitMQConfiguration _config;
    private readonly RabbitMQService _mqService;

    public Consumer(RabbitMQService rabbitMqService, IRabbitMQConfiguration config, IFakeERPClient erpClient, ILogger<Consumer> logger, RabbitMQService mqService)
    {
        _config = config;
        _erpClient = erpClient;
        _logger = logger;
        _mqService = mqService;
    }

    public async Task OpenQueueAsync(CancellationToken cancellationToken)
    {
        bool erpSuccess = false;
        ulong deliveryTag = 0;

        var channelConnection = await _mqService.DeclareDurableQueueAsync();

        var consumer = new AsyncEventingBasicConsumer(channelConnection.channel);
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
            await channelConnection.channel.BasicRejectAsync(
                deliveryTag: deliveryTag,
                requeue: false,
                cancellationToken: cancellationToken);
        }
        else
        {
            await channelConnection.channel.BasicAckAsync(
                deliveryTag: deliveryTag,
                multiple: false,
                cancellationToken: cancellationToken);
        }

    }
}
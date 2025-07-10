using System.Text;
using System.Text.Json;
using IntegrationSimulator.IntegrationService.Domain.Interfaces;
using IntegrationSimulator.IntegrationService.Infrastructure.MessageQueueing;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Threading;
using IntegrationSimulator.IntegrationService.Domain.Models;
using IntegrationSimulator.IntegrationService.Domain.Models.Results;
using System.Threading.Channels;

namespace IntegrationSimulator.IntegrationService.Program.HostedServices;

public class FakeERPDeQueueingService : BackgroundService
{
    private readonly IFakeERPClient _erpClient;
    private readonly ILogger<FakeERPDeQueueingService> _logger;
    private readonly IRabbitMQConfiguration _config;
    private readonly IConnectionFactory _factory;
    private IConnection? _connection;
    private IChannel? _channel;

    public FakeERPDeQueueingService(IFakeERPClient erpClient, ILogger<FakeERPDeQueueingService> logger, IRabbitMQConfiguration config, IConnectionFactory factory)
    {
        _erpClient = erpClient;
        _logger = logger;
        _config = config;
        _factory = factory;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        bool open = false;

        while (!cancellationToken.IsCancellationRequested)
        {
            if (!open)
            {
                _connection = await _factory.CreateConnectionAsync(cancellationToken);
                _channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

                await _channel.QueueDeclareAsync(
                    queue: nameof(FakeERPDeQueueingService),
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    cancellationToken: cancellationToken);

                await _channel.ExchangeDeclareAsync(
                    exchange: _config.ExchangeName,
                    type: ExchangeType.Fanout,
                    durable: true,
                    autoDelete: false,
                    cancellationToken: cancellationToken);

                await _channel.QueueBindAsync(
                    queue: nameof(FakeERPDeQueueingService),
                    exchange: _config.ExchangeName,
                    routingKey: "",
                    cancellationToken: cancellationToken);

                var consumer = new AsyncEventingBasicConsumer(_channel);

                consumer.ReceivedAsync += OnReceivedAsync;


                await _channel.BasicConsumeAsync(
                    queue: nameof(FakeERPDeQueueingService),
                    autoAck: false,
                    consumer: consumer);

                open = true;
            }
        }

    }

    private async Task OnReceivedAsync(object obj, BasicDeliverEventArgs eventArgs)
    {
        var body = eventArgs.Body.ToArray();
        var jsonString = Encoding.UTF8.GetString(body);
        var deliveryTag = eventArgs.DeliveryTag;

        //TODO: Crashing the consumer with an exception is probably a bad idea. Find a better way
        var ads = JsonSerializer.Deserialize<QueueAdsDto>(jsonString) ?? throw new JsonException($"Failed to deserialize json: {jsonString} to: {nameof(QueueAdsDto)}");

        _logger.LogInformation("Trace: {id}. Received {number} ads from queue. Processing...", ads.Trace, ads.AdsData.NumberOfAds);

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

        _logger.LogInformation("Trace: {id}. AdsData sent to fakeERP: {numOfAds}", ads.Trace, ads.AdsData.NumberOfAds);

        await ((AsyncEventingBasicConsumer)obj).Channel.BasicAckAsync(
            deliveryTag: deliveryTag,
            multiple: false);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            await _channel.CloseAsync(cancellationToken);
        }

        if (_connection is not null)
        {
            await _connection.CloseAsync(cancellationToken);
        }

        await base.StopAsync(cancellationToken);
    }
}
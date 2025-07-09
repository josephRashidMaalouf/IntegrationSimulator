using IntegrationSimulator.IntegrationService.Domain.Interfaces;
using IntegrationSimulator.IntegrationService.Domain.Models;
using IntegrationSimulator.IntegrationService.Domain.Models.Results;
using Microsoft.Extensions.Logging;

namespace IntegrationSimulator.IntegrationService.Application.Services;
public class PollingCoordinator : IPollingCoordinator
{
    private readonly IFakeERPClient _erpClient;
    private readonly IJobAdClient _jobAdClient;
    private readonly IMetaDataRepository _metaDataRepository;
    private readonly ILogger<PollingCoordinator> _logger;
    private readonly IProducer _producer;

    public PollingCoordinator(IFakeERPClient erpClient, IJobAdClient jobAdClient, ILogger<PollingCoordinator> logger, IMetaDataRepository metaDataRepository, IProducer producer)
    {
        _erpClient = erpClient;
        _jobAdClient = jobAdClient;
        _logger = logger;
        _metaDataRepository = metaDataRepository;
        _producer = producer;
    }

    public async Task Execute(Guid trace)
    {
        DateTime latestSuccessfullFetch = await _metaDataRepository.GetLatestSuccessfulFetchDate();

        var result = await _jobAdClient.GetNewAdListingsAsync(latestSuccessfullFetch, trace);

        if (result is not SuccessResult<GetAdsResponse> successResult)
        {
            var errorResult = (ErrorResult<GetAdsRequest>)result;

            foreach (var error in errorResult.Errors)
            {
                _logger.LogWarning("Trace: {trace}. " + error.Message, error.Trace);

            }
            return;
        }

        var ads = successResult.Data;


        await _producer.PublishToQueueAsync(new QueueAdsDto(ads, trace));
        
        _logger.LogInformation("Trace: {id}. New job ads listed: {numOfAds}. Sent to queue.", trace, ads.NumberOfAds);

        var mostRecentAdDate = ads.Ads
            .OrderByDescending(x => x.PublishedDate)
            .First()
            .PublishedDate;

        await _metaDataRepository.SaveMostRecentSavedAdDateAsync(trace, mostRecentAdDate);

    }
}

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

    public PollingCoordinator(IFakeERPClient erpClient, IJobAdClient jobAdClient, ILogger<PollingCoordinator> logger, IMetaDataRepository metaDataRepository)
    {
        _erpClient = erpClient;
        _jobAdClient = jobAdClient;
        _logger = logger;
        _metaDataRepository = metaDataRepository;
    }

    public async Task<Result> Execute(Guid trace)
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
            return result;
        }

        var ads = successResult.Data;

        _logger.LogInformation("Trace: {id}. New job ads listed: {numOfAds}", trace, ads.NumberOfAds);

        if (ads.NumberOfAds != 0)
        {
            var dto = ads.Ads
                .Select(x => new PostNewJobToFakeERP(x.Id, x.Title, x.WorkplaceName, x.PublishedDate))
                .ToList();
            var resultErp = await _erpClient.PostNewJobListingsAsync(dto, trace);

            if (resultErp is ErrorResult<List<PostNewJobToFakeERP>> erpErrorResult)
            {

                foreach (var er in erpErrorResult.Errors)
                {
                    _logger.LogWarning("Trace: {id}. Could not send ads to fake ERP: {reason}", er.Trace, er.Message);
                }

                return erpErrorResult;
            }

            var mostRecentAdDate = ads.Ads
                .OrderByDescending(x => x.PublishedDate)
                .First()
                .PublishedDate;

            await _metaDataRepository.SaveMostRecentSavedAdDateAsync(trace, mostRecentAdDate);
        }
        _logger.LogInformation("Trace: {id}. Ads sent to fakeERP: {numOfAds}", trace, ads.NumberOfAds);
        
        return new SuccessResult<GetAdsResponse>(ads);
    }
}
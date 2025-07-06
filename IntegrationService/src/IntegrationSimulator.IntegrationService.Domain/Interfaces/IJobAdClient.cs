using IntegrationSimulator.IntegrationService.Domain.Models;

namespace IntegrationSimulator.IntegrationService.Domain.Interfaces;

public interface IJobAdClient
{
    Task<GetAdsResponse?> GetNewAdListingsAsync(DateTime latestedFetchedAdsDate);
}

public interface IFakeERPClient
{
    Task PostNewJobListingsAsync(PostNewJobToFakeERP dto);
}
using IntegrationSimulator.IntegrationService.Domain.Models;
using IntegrationSimulator.IntegrationService.Domain.Models.Results;

namespace IntegrationSimulator.IntegrationService.Domain.Interfaces;

public interface IJobAdClient
{
    Task<Result> GetNewAdListingsAsync(DateTime latestFetchedAdsDate, Guid trace);
}
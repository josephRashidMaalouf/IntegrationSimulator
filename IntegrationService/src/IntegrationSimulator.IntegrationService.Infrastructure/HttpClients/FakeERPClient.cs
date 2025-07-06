using IntegrationSimulator.IntegrationService.Domain.Interfaces;
using IntegrationSimulator.IntegrationService.Domain.Models;

namespace IntegrationSimulator.IntegrationService.Infrastructure.HttpClients;

public class PlatsbankenClient : IJobAdClient
{
    private readonly HttpClient _httpClient;

    public PlatsbankenClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://platsbanken-api.arbetsformedlingen.se/jobs/v1/search");
    }

    public Task<GetAdsResponse?> GetNewAdListingsAsync(DateTime latestedFetchedAdsDate)
    {
        throw new NotImplementedException();
    }
}

public class FakeERPClient : IFakeERPClient
{
    private readonly HttpClient _httpClient;

    public FakeERPClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://localhost:7235");
    }

    public Task PostNewJobListingsAsync(PostNewJobToFakeERP dto)
    {
        throw new NotImplementedException();
    }
}
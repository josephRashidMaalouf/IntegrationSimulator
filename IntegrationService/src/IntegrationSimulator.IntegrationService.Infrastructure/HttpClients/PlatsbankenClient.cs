using System.Net.Http.Json;
using IntegrationSimulator.IntegrationService.Constants;
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

    public async Task<GetAdsResponse> GetNewAdListingsAsync(DateTime latestedFetchedAdsDate)
    {
        var dto = new GetAdsRequest()
        {
            Filters =
            [
                new Filter
                {
                    Type = PlatsbankenFilterQueryConstants.TypeFreeText,
                    Value = PlatsbankenFilterQueryConstants.ValueDotNet
                },
                new Filter
                {
                    Type = PlatsbankenFilterQueryConstants.TypeMunicipality,
                    Value = PlatsbankenFilterQueryConstants.ValueGothenburg
                }
            ],
            FromDate = latestedFetchedAdsDate.AddSeconds(1)
        };

        var result = await _httpClient.PostAsJsonAsync<GetAdsRequest>("", dto);
        
        if (!result.IsSuccessStatusCode)
        {
            //TODO: Implement retry, log and handle this somehow
        }

        //TODO: implement result pattern to handle null reference returns
        return await result.Content.ReadFromJsonAsync<GetAdsResponse>();




    }
}
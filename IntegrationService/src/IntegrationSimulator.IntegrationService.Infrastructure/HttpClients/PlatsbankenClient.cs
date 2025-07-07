using System.Net.Http.Json;
using IntegrationSimulator.IntegrationService.Constants;
using IntegrationSimulator.IntegrationService.Domain.Interfaces;
using IntegrationSimulator.IntegrationService.Domain.Models;
using IntegrationSimulator.IntegrationService.Domain.Models.Results;
using Microsoft.Extensions.Configuration;

namespace IntegrationSimulator.IntegrationService.Infrastructure.HttpClients;

public class PlatsbankenClient : IJobAdClient
{
    private readonly HttpClient _httpClient;

    public PlatsbankenClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        var endpoints = config.GetSection("Endpoints");
        var uri = endpoints["Platsbanken"] ?? "";
        _httpClient.BaseAddress = new Uri(uri);
        _httpClient.BaseAddress = new Uri(uri);
    }

    public async Task<Result> GetNewAdListingsAsync(DateTime latestFetchedAdsDate, Guid trace)
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
            FromDate = latestFetchedAdsDate.AddSeconds(1)
        };

        var result = await _httpClient.PostAsJsonAsync<GetAdsRequest>("", dto);
        
        if (!result.IsSuccessStatusCode)
        {
            return new ErrorResult<GetAdsRequest>(dto, new Error()
            {
                Trace = trace,
                Message = $"Could not reach platsbanken api on: {_httpClient.BaseAddress?.AbsoluteUri ?? "[no uri]"}"
            });
        }

        var data = await result.Content.ReadFromJsonAsync<GetAdsResponse>();

        return new SuccessResult<GetAdsResponse?>(data);
    }
}
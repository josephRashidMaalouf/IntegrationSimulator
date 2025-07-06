using System.Net.Http.Json;
using IntegrationSimulator.IntegrationService.Domain.Interfaces;
using IntegrationSimulator.IntegrationService.Domain.Models;

namespace IntegrationSimulator.IntegrationService.Infrastructure.HttpClients;

public class FakeERPClient : IFakeERPClient
{
    private readonly HttpClient _httpClient;

    public FakeERPClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("http://localhost:5000");
    }

    public async Task PostNewJobListingsAsync(List<PostNewJobToFakeERP> dto)
    {
        var result = await _httpClient.PostAsJsonAsync<List<PostNewJobToFakeERP>>("", dto);

        if (!result.IsSuccessStatusCode)
        {
            //TODO: Log, handle, retry
        }

        //TODO: Probably return something to indicate success? Maybe not.. We will see.
    }
}
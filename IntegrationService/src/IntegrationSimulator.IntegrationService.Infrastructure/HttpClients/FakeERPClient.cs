using System.Net.Http.Json;
using IntegrationSimulator.IntegrationService.Domain.Interfaces;
using IntegrationSimulator.IntegrationService.Domain.Models;
using IntegrationSimulator.IntegrationService.Domain.Models.Results;

namespace IntegrationSimulator.IntegrationService.Infrastructure.HttpClients;

public class FakeERPClient : IFakeERPClient
{
    private readonly HttpClient _httpClient;

    public FakeERPClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("http://localhost:5000");
    }

    public async Task<Result> PostNewJobListingsAsync(List<PostNewJobToFakeERP> dto, Guid trace)
    {
        var result = await _httpClient.PostAsJsonAsync<List<PostNewJobToFakeERP>>("", dto);

        if (!result.IsSuccessStatusCode)
        {
            return new ErrorResult<List<PostNewJobToFakeERP>>(dto, new Error()
            {
                Trace = trace,
                Message = "Could not reach fakeERP. No data has been submitted"
            });
        }

        return new SuccessResult<List<PostNewJobToFakeERP>>(dto);
    }

}
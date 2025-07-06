using IntegrationSimulator.IntegrationService.Domain.Models;
using IntegrationSimulator.IntegrationService.Domain.Models.Results;

namespace IntegrationSimulator.IntegrationService.Domain.Interfaces;

public interface IFakeERPClient
{
    Task<Result> PostNewJobListingsAsync(List<PostNewJobToFakeERP> dto, Guid trace);
}
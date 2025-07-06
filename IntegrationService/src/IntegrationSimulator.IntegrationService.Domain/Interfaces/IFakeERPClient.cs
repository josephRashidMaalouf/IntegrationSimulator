using IntegrationSimulator.IntegrationService.Domain.Models;

namespace IntegrationSimulator.IntegrationService.Domain.Interfaces;

public interface IFakeERPClient
{
    Task PostNewJobListingsAsync(List<PostNewJobToFakeERP> dto);
}
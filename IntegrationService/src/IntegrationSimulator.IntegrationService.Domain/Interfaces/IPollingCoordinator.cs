using IntegrationSimulator.IntegrationService.Domain.Models.Results;

namespace IntegrationSimulator.IntegrationService.Domain.Interfaces;

public interface IPollingCoordinator
{
    Task Execute(Guid trace);
}
namespace IntegrationSimulator.IntegrationService.Domain.Interfaces;

public interface IMetaDataRepository
{
    Task<DateTime> GetLatestSuccessfulFetchDate();

    Task SaveMostRecentSavedAdDateAsync(Guid trace, DateTime date);
}
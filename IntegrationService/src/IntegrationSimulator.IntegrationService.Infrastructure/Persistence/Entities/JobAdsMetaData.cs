namespace IntegrationSimulator.IntegrationService.Infrastructure.Persistence.Entities;

public class JobAdsMetaData
{
    public required Guid Id { get; set; }
    public required DateTime LatestSuccessfulFetch { get; set; }
}
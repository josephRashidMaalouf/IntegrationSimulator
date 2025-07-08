using IntegrationSimulator.IntegrationService.Domain.Interfaces;
using IntegrationSimulator.IntegrationService.Infrastructure.Persistence.Entities;

namespace IntegrationSimulator.IntegrationService.Infrastructure.Persistence.Repositories;

public class MetaDataRepository : IMetaDataRepository
{
    private readonly JobAdsMetaDataContext _context;

    public MetaDataRepository(JobAdsMetaDataContext context)
    {
        _context = context;
    }

    public Task<DateTime> GetLatestSuccessfulFetchDate()
    {
        var latest = _context.MetaData
            .OrderByDescending(x => x.LatestSuccessfulFetch)
            .FirstOrDefault();

        if (latest is null)
        {
            return Task.FromResult(DateTime.UtcNow.AddDays(-7));
        }

        var utcTime = DateTime.SpecifyKind(latest.LatestSuccessfulFetch, DateTimeKind.Utc);

        return Task.FromResult(utcTime);
    }

    public async Task SaveMostRecentSavedAdDateAsync(Guid trace, DateTime date)
    {
        await _context.MetaData.AddAsync(new JobAdsMetaData()
        {
            Id = trace,
            LatestSuccessfulFetch = date
        });

        await _context.SaveChangesAsync();
    }
}
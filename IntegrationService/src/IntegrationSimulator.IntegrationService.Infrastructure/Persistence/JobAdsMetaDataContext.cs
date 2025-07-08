using IntegrationSimulator.IntegrationService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace IntegrationSimulator.IntegrationService.Infrastructure.Persistence;

public class JobAdsMetaDataContext : DbContext
{
    public DbSet<JobAdsMetaData> MetaData => Set<JobAdsMetaData>();

    private readonly string _path;
    public JobAdsMetaDataContext()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Db");

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        _path = Path.Combine(path, "integrationSimulationJobAdsMetaData.db");
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={_path}");
}
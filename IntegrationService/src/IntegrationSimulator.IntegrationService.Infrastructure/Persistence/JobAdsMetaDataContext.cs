using IntegrationSimulator.IntegrationService.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace IntegrationSimulator.IntegrationService.Infrastructure.Persistence;

public class JobAdsMetaDataContext : DbContext
{
    public DbSet<JobAdsMetaData> MetaData => Set<JobAdsMetaData>();

    private readonly string _path;
    public JobAdsMetaDataContext()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var spec = Environment.GetFolderPath(folder);
        var path = Path.Combine(spec, "JobAdsMetadataDb");

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        _path = Path.Join(path, "integrationSimulationJobAdsMetaData.db");
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={_path}");
}
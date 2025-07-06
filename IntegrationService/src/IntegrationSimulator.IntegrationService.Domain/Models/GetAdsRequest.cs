namespace IntegrationSimulator.IntegrationService.Domain.Models;


public class GetAdsRequest
{
    public required Filter[] Filters { get; set; }
    public required DateTime FromDate { get; set; }
    public string Order { get; } = "date";
    public int MaxRecords { get; } = 50;
    public int StartIndex { get; } = 0;
    public DateTime ToDate { get; } = DateTime.UtcNow;
    public string Source { get; } = "pb";
}

public class Filter
{
    public required string Type { get; set; }
    public required string Value { get; set; }
}

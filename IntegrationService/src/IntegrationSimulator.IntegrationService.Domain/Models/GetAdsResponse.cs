namespace IntegrationSimulator.IntegrationService.Domain.Models;

public class GetAdsResponse
{
    public int NumberOfAds { get; set; }
    public Ad[] Ads { get; set; }
}

public class Ad
{
    public required string Id { get; set; }
    public required DateTime PublishedDate { get; set; }
    public required DateTime LastApplicationDate { get; set; }
    public required string Title { get; set; }
    public required string Occupation { get; set; }
    public required string Workplace { get; set; }
    public required string WorkplaceName { get; set; }
    public required bool Published { get; set; }
    public required int Positions { get; set; }
}

namespace IntegrationSimulator.FakeERP;

public class JobPostedRequestDto(string Id, string Title, string WorkplaceName, DateTime PublicationTime)
{
    public string Id { get; init; } = Id;
    public string Title { get; init; } = Title;
    public string WorkplaceName { get; init; } = WorkplaceName;
    public DateTime PublicationTime { get; init; } = PublicationTime;

    public override string ToString()
    {
        return $"{Id},{Title},{WorkplaceName},{PublicationTime}";
    }
}
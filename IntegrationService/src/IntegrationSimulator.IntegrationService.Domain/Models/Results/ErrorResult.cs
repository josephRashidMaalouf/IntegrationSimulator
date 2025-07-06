namespace IntegrationSimulator.IntegrationService.Domain.Models.Results;

public class ErrorResult : Result
{
    public List<Error> Errors { get; set; } = [];
    public ErrorResult(params Error[] error)
    {
        Success = false;
        Errors.AddRange(error);
    }
}

public class ErrorResult<T> : Result<T>
{
    public List<Error> Errors { get; set; } = [];
    public ErrorResult(T data, params Error[] error) : base(data)
    {
        Success = false;
        Errors.AddRange(error);
    }
}

public class Error()
{
    public required string Message { get; set; }
    public required Guid Trace { get; set; }
}
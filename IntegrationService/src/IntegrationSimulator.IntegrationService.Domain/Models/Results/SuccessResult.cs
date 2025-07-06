namespace IntegrationSimulator.IntegrationService.Domain.Models.Results;

public class SuccessResult : Result
{
    public SuccessResult()
    {
        Success = true;

    }

    public SuccessResult(string message) 
    {
        Success = true;

    }
}

public class SuccessResult<T> : Result<T>
{
    public SuccessResult(T data) : base(data)
    {
        Success = true;
    }
}
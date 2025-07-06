namespace IntegrationSimulator.IntegrationService.Domain.Models.Results;

public abstract class Result
{
    public bool Success { get; protected set; }
}

public abstract class Result<T> : Result
{
    public T Data { get; set; }
    
    protected Result(T data)
    {
        Data = data;
    }

}
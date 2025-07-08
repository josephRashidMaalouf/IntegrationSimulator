using Polly;
using Polly.Extensions.Http;

namespace IntegrationSimulator.IntegrationService.Infrastructure.PollyHandlers;

public  class RetryPolicy
{
    public static IAsyncPolicy<HttpResponseMessage> Get()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                retryAttempt)));
    }
}
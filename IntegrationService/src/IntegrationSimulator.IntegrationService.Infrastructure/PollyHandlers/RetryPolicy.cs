using Polly;
using Polly.Extensions.Http;

namespace IntegrationSimulator.IntegrationService.Infrastructure.PollyHandlers;

public  class RetryPolicy
{
    static IAsyncPolicy<HttpResponseMessage> Get()
    {
        //TODO: remove the notfound line
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                retryAttempt)));
    }
}
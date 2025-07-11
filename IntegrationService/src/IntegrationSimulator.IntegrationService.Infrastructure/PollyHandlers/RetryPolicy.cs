using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;

namespace IntegrationSimulator.IntegrationService.Infrastructure.PollyHandlers;

public class RetryPolicy
{

    public static IAsyncPolicy<HttpResponseMessage> HttpPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                retryAttempt)));
    }

    public static AsyncRetryPolicy QueueBindingPolicy<T>(ILogger<T> logger)
    {
        return Policy
            .Handle<Exception>() // or a more specific one
            .WaitAndRetryForeverAsync(
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetryAsync: async (exception, delay) =>
                {
                    // Optional: log retry attempt
                    logger.LogWarning($"Retrying in {delay}.");
                    await Task.CompletedTask;
                });
    }
}
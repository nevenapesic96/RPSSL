using System.Net;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Infrastructure.ApiClients;

public abstract class ApiClientBase
{
    private const int MaxRetryAttempts = 2;

    protected readonly AsyncRetryPolicy<HttpResponseMessage> RetryPolicy;

    protected ApiClientBase(ILogger logger)
    {
        RetryPolicy = Policy.Handle<Exception>()
            .OrResult<HttpResponseMessage>(r =>
                r.StatusCode >= HttpStatusCode.InternalServerError)
            .WaitAndRetryAsync(MaxRetryAttempts, retryAttempt =>
                {
                    var sleepDuration = TimeSpan.FromSeconds(Math.Pow(3, retryAttempt));
                    logger.LogWarning("Api call: Retrying in {SleepDuration}. Attempt: {RetryAttempt}/{MaxRetryAttempts}", sleepDuration, retryAttempt, MaxRetryAttempts);
                
                    return sleepDuration;
                },
                (msg, _, _) =>
                {
                    if (msg.Exception is not null)
                        logger.LogError("The retry failed causing an exception: {Exception}", msg.Exception);

                    if (msg.Result is not null)
                        logger.LogError("The retry failed with an unexpected HTTP response. Url: {Url}, HttpStatus: {HttpStatus}", msg.Result?.RequestMessage?.RequestUri?.AbsoluteUri, msg.Result?.StatusCode);
                });
    }
}
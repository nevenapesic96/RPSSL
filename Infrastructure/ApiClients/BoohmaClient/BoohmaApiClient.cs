using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace Infrastructure.ApiClients.BoohmaClient;

public interface IBoohmaApiClient
{
    /// <summary>
    /// Method for fetching random number
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<RandomNumberResponse> GetRandomNumber(CancellationToken cancellationToken);
}

public class BoohmaApiApiClient(HttpClient client, ILogger<BoohmaApiApiClient> logger) : ApiClientBase(logger), IBoohmaApiClient
{

    public async Task<RandomNumberResponse> GetRandomNumber(CancellationToken cancellationToken)
    {
        logger.LogInformation("Calling BoohmaClient api to fetch random number");

        var responseData = await RetryPolicy.ExecuteAsync(async _ => await client.GetAsync("/random", cancellationToken), cancellationToken);
        var response = await responseData.Content.ReadFromJsonAsync<RandomNumberResponse>(cancellationToken: cancellationToken);

        if (response is not null) return response;

        logger.LogWarning("Unable to fetch from BoohmaClient. Falling back to internal random generator");
        
        return new RandomNumberResponse
        {
            RandomNumber = new Random().Next(1, 6)
        };
    }
}
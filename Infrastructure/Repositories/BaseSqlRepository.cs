using System.Collections.Immutable;
using Dapper;
using Domain;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;
using Polly.Retry;

namespace Infrastructure.Repositories;

public class BaseSqlRepository
{
    private const int MaxRetryAttempts = 3;
    
    private readonly AsyncRetryPolicy _retrySqlPolicy;
    private readonly AppSettings _appSettings;

    protected BaseSqlRepository(ILogger logger, AppSettings appSettings)
    {
        _appSettings = appSettings;

        _retrySqlPolicy = Policy.Handle<NpgsqlException>()
            .WaitAndRetryAsync(MaxRetryAttempts, retryAttempt =>
                {
                    var sleepDuration = TimeSpan.FromSeconds(Math.Pow(3, retryAttempt));
                    logger.LogWarning(
                        "PostgresQuery: Retrying in {SleepDuration}. Attempt: {RetryAttempt}/{MaxRetryAttempts}",
                        sleepDuration, retryAttempt, MaxRetryAttempts);
                    return sleepDuration;
                },
                (ex, _, _) =>
                    logger.LogError(ex, "Retry failed. Error: {Exception}", ex)
            );
    }
    
    /// <summary>
    /// Method used for executing queries
    /// </summary>
    /// <param name="query">Query that should be executed</param>
    /// <param name="parameters">Parameters needed to execute specified query</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
    /// <returns>Number of rows affected</returns>
    protected async Task<int> ExecuteScalar(string query, object parameters, CancellationToken cancellationToken)
    {
        return await _retrySqlPolicy.ExecuteAsync(async _ =>
        {
            await using var connection = new NpgsqlConnection(_appSettings.PostgresServerSettings.ConnectionString);

            return await connection.ExecuteAsync(query, parameters);
        }, cancellationToken);
    }
    
    /// <summary>
    /// Method that returns list of records fetched with passed query
    /// </summary>
    /// <param name="query">Query for fetching data</param>
    /// <param name="parameters">Parameters needed to execute specified query</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
    /// <typeparam name="T">Type of result object</typeparam>
    /// <returns></returns>
    protected async Task<IEnumerable<T>> ExecuteQueryList<T>(string query, object parameters, CancellationToken cancellationToken)
    {
        return await _retrySqlPolicy.ExecuteAsync(async _ =>
        {
            await using var connection = new NpgsqlConnection(_appSettings.PostgresServerSettings.ConnectionString);

            var result = await connection.QueryAsync<T>(new CommandDefinition(query, parameters,
                cancellationToken: cancellationToken));

            return result.ToImmutableList();
        }, cancellationToken);
    }
}
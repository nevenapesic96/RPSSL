using Domain;
using Domain.Models;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public interface IPlayRepository
{
    /// <summary>
    /// Method that saves the game result in table
    /// </summary>
    /// <param name="userName">Name of user that played the game</param>
    /// <param name="result">Result of the played game</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
    /// <returns>Result of the operation</returns>
    Task<bool> SaveGame(string userName, string result, CancellationToken cancellationToken);
    
    /// <summary>
    /// Method that deletes all record from table
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
    /// <returns>Result of the operation</returns>
    Task<bool> DeleteAll(CancellationToken cancellationToken);
    
    /// <summary>
    /// Method that returns list of latest results
    /// </summary>
    /// <param name="count">Number of results that should be returned</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
    /// <returns>List of game results</returns>
    Task<IEnumerable<PlayResult>> GetLastResults(int count, CancellationToken cancellationToken);
    
    /// <summary>
    /// Method that returns list of all results for specified user
    /// </summary>
    /// <param name="username">Name of user that played the game</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
    /// <returns>List of game results</returns>
    Task<IEnumerable<PlayResult>> GetResultsForUsername(string username, CancellationToken cancellationToken);
    
    /// <summary>
    /// Method that deletes all results for specified user
    /// </summary>
    /// <param name="username">Name of user that played the game</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
    /// <returns>Result of the operation</returns>
    Task<bool> DeleteForUser(string username, CancellationToken cancellationToken);
}

public class PlayRepository(ILogger<PlayRepository> logger, AppSettings appSettings)
    : BaseSqlRepository(logger, appSettings), IPlayRepository
{
    public async Task<bool> SaveGame(string userName, string result, CancellationToken cancellationToken)
    {
        logger.LogInformation("Saving result for {Username}", userName);

        const string query = "INSERT INTO public.play_results (userName, playTime, result) VALUES (@UserName, @PlayTime, @Result)";
        
        var playTime = DateTime.UtcNow;
        var parameters = new
        {
            userName,
            playTime,
            result
        };

        var saveResult = await ExecuteScalar(query, parameters, cancellationToken) == 1;

        logger.LogInformation("Finished saving result for {Username}. Operation result - {SaveResult}", userName, saveResult);

        return saveResult;
    }

    public async Task<IEnumerable<PlayResult>> GetLastResults(int count, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching last {Count} results", count);
        
        const string query = "SELECT * FROM play_results ORDER BY playTime DESC LIMIT @limit";

        var parameters = new { limit = count };

        return await ExecuteQueryList<PlayResult>(query, parameters, cancellationToken);
    }

    public async Task<IEnumerable<PlayResult>> GetResultsForUsername(string username, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching results for user {Username}", username);
        
        const string query = "SELECT * FROM play_results WHERE username=@Username";

        return await ExecuteQueryList<PlayResult>(query, new { username }, cancellationToken);
    }

    public async Task<bool> DeleteAll(CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting all results");

        const string query = "DELETE FROM play_results";

        var deleteResult = await ExecuteScalar(query, new { }, cancellationToken) > 0;

        logger.LogInformation("Finished deleting all results. Operation result - {DeleteResult}", deleteResult);

        return deleteResult;
    }

    public async Task<bool> DeleteForUser(string username, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting results for user {Username}", username);

        const string query = "DELETE FROM play_results WHERE userName = @Username";

        var deleteResult = await ExecuteScalar(query, new { username}, cancellationToken) > 0;

        logger.LogInformation("Finished deleting results for user {Username}. Operation result - {DeleteResult}", username, deleteResult);

        return deleteResult;
    }
}
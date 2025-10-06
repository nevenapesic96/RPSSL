using Application.Common;
using Application.DTOs;
using AutoMapper;
using Domain;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public interface IPlayService
{
    /// <summary>
    /// Method used for calculating result in game between computer and user. Result is permanently saved
    /// </summary>
    /// <param name="request">Object that contains details about opponent moves</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
    /// <returns>Result of the played game</returns>
    Task<OperationResult<PlayResponse>> Play(PlayRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Method for deleting stored results, for single user, or all
    /// </summary>
    /// <param name="username">User for which results should be deleted. If empty, all results will be deleted</param>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
    /// <returns>Result of delete operation</returns>
    Task<OperationResult<PlayResult>> ResetResults(string?  username, CancellationToken cancellationToken);

    /// <summary>
    /// Method for fetching latest result. Number of results is configured in appsettings
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
    /// <returns>List of latest results with all details</returns>
    Task<IEnumerable<ResultResponse>> GetLatestResults(CancellationToken cancellationToken);
}

public class PlayService(IChoicesService choicesService, IPlayRepository playRepository, AppSettings appSettings, IMapper mapper, ILogger<PlayService> logger) : IPlayService
{
    public async Task<OperationResult<PlayResponse>> Play(PlayRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Playing game for user {Username}", request.Username);
        var computerChoice = await choicesService.GetValidRandomChoice(cancellationToken);

        var gameResult = CalculateResult((Choices)request.PlayerChoice, computerChoice);
        
        logger.LogInformation("Result of the game: {GameResult}. Player choice: {Player}; Computer choice: {Computer}", gameResult.ToString(), ((Choices)request.PlayerChoice).ToString(), computerChoice.ToString());
        var saveResult = await playRepository.SaveGame(request.Username, gameResult.ToString(), cancellationToken);

        return new OperationResult<PlayResponse>
        {
            IsSuccessful = saveResult,
            Result = new PlayResponse
            {
                Results = gameResult,
                Computer = (int)computerChoice,
                Player = request.PlayerChoice
            }
        };
    }

    public async Task<OperationResult<PlayResult>> ResetResults(string? username, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrEmpty(username)) return await ResetResultsForUser(username, cancellationToken);

        logger.LogInformation("Resetting all results");
        var result = await playRepository.DeleteAll(cancellationToken);
        if (result) return new OperationResult<PlayResult> { IsSuccessful = true };

        logger.LogError("Error happened while trying to delete all results");
        return new OperationResult<PlayResult>
        {
            IsSuccessful = false, Error = new ApplicationError
            {
                Type = ApplicationErrorType.UnprocessableEntity
            }
        };
    }

    private async Task<OperationResult<PlayResult>> ResetResultsForUser(string username, CancellationToken cancellationToken)
    {
        logger.LogInformation("Resetting results for user {Username}", username);
        
        var existingResults = await playRepository.GetResultsForUsername(username, cancellationToken);
        if (!existingResults.Any())
            return new OperationResult<PlayResult>
            {
                IsSuccessful = false,
                Error = new ApplicationError
                {
                    Type = ApplicationErrorType.NotFound
                }
            };

        var result = await playRepository.DeleteForUser(username, cancellationToken);
        if (result) return new OperationResult<PlayResult> { IsSuccessful = true };

        logger.LogError("Error happened while trying to delete results for user {Username}", username);
        return new OperationResult<PlayResult>
        {
            IsSuccessful = false, Error = new ApplicationError
            {
                Type = ApplicationErrorType.UnprocessableEntity
            }
        };
    }

    public async Task<IEnumerable<ResultResponse>> GetLatestResults(CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching latest results");
        
        var latestCount = appSettings.LatestResultsCount;
        var results = await playRepository.GetLastResults(latestCount, cancellationToken);

        return mapper.Map<IEnumerable<ResultResponse>>(results);
    }

    private static GameResult CalculateResult(Choices playerChoice, Choices computerChoice)
    {
        if (playerChoice == computerChoice) return GameResult.Tie;

        return PlayRules.Rules[playerChoice].Contains(computerChoice) ? GameResult.Win : GameResult.Lose;
    }
}
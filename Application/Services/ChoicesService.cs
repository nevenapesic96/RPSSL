using Application.DTOs;
using AutoMapper;
using Domain.Enums;
using Infrastructure.ApiClients.BoohmaClient;
using Infrastructure.Helpers;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public interface IChoicesService
{
    /// <summary>
    /// Returns all possible choices in game
    /// </summary>
    /// <returns>List of all choices</returns>
    IEnumerable<ChoiceResponse> GetAllChoices();
    
    /// <summary>
    /// Returns one random choice from all possible choices
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
    /// <returns>Object that contains choice details</returns>
    Task<ChoiceResponse> GetRandomChoice(CancellationToken cancellationToken);
    
    /// <summary>
    /// Returns one random choice from all possible choices
    /// </summary>
    /// <param name="cancellationToken">Propagates notification that operations should be canceled</param>
    /// <returns>Enum choice value</returns>
    Task<Choices> GetValidRandomChoice(CancellationToken cancellationToken);
}

public class ChoicesService(IBoohmaApiClient boohmaApiClient, IMapper mapper, ILogger<IChoicesService> logger):IChoicesService
{
    public IEnumerable<ChoiceResponse> GetAllChoices()
    {
        logger.LogInformation("Retrieving all choices");
        
        return mapper.Map<IEnumerable<ChoiceResponse>>(ChoiceHelpers.AllChoices());
    }

    public async Task<Choices> GetValidRandomChoice(CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving valid random choice");
        
        var randomNumber = await boohmaApiClient.GetRandomNumber(cancellationToken);

        return (Choices)(randomNumber.RandomNumber % ChoiceHelpers.AllChoicesCount()) + 1;
    }

    public async Task<ChoiceResponse> GetRandomChoice(CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving valid random choice");
        
        var randomChoice = await GetValidRandomChoice(cancellationToken);

        return mapper.Map<ChoiceResponse>(randomChoice);
    }
}
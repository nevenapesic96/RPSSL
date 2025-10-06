using Domain.Enums;

namespace Application.DTOs;

/// <summary>
/// Object that contains all details about play
/// </summary>
public record PlayResponse
{
    /// <summary>
    /// Result of the game
    /// </summary>
    public GameResult Results { get; init; }
    
    /// <summary>
    /// Identifier of choice that player played
    /// </summary>
    public int Player { get; init; }
    
    /// <summary>
    /// Identifier of choice that computer played
    /// </summary>
    public int Computer { get; init; }
}
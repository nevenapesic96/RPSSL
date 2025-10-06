using Domain.Enums;

namespace Application.DTOs;

/// <summary>
/// Object that contains all data about result of the game
/// </summary>
public record ResultResponse
{
    /// <summary>
    /// Time when game was played, in UTC format
    /// </summary>
    public DateTime PlayTime { get; init; }
    
    /// <summary>
    /// Username of player in game
    /// </summary>
    public required string Username { get; init; }
    
    /// <summary>
    /// Result of the game that was played
    /// </summary>
    public GameResult Result { get; init; }
}
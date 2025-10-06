namespace Domain.Models;

/// <summary>
/// Object that contains details about played game
/// </summary>
public class PlayResult
{
    /// <summary>
    /// Identifier for user that played the game
    /// </summary>
    public required string Username { get; init; }
    
    /// <summary>
    /// Time when game was played, in UTC format
    /// </summary>
    public DateTime PlayTime { get; init; }
    
    /// <summary>
    /// Result of the played game
    /// </summary>
    public required string Result { get; init; }
}
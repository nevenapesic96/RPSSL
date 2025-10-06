using System.Text.Json.Serialization;

namespace Domain.Enums;

/// <summary>
/// List of possible game results
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum GameResult
{
    Win,
    Lose,
    Tie
}
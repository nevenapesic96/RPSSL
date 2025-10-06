using System.Text.Json.Serialization;

namespace Infrastructure.ApiClients.BoohmaClient;

/// <summary>
/// Object that endpoint returns
/// </summary>
public record RandomNumberResponse
{
    /// <summary>
    /// Random number
    /// </summary>
    [JsonPropertyName("random_number")]
    public int RandomNumber { get; init; }
}
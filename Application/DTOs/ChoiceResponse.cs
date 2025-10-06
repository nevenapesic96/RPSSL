namespace Application.DTOs;

/// <summary>
/// Object that contains all details about choice
/// </summary>
public record ChoiceResponse{
    
    /// <summary>
    /// Identifier of choice
    /// </summary>
    public int Id { get; init; }
    
    /// <summary>
    /// Name of choice
    /// </summary>
    public required string Name { get; init; }
}
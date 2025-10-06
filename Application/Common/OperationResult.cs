namespace Application.Common;

/// <summary>
/// Object that contains details about operation succession
/// </summary>
/// <typeparam name="T">Result payload type</typeparam>
public record OperationResult<T>
{
    /// <summary>
    /// Object that contains result payload
    /// </summary>
    public T? Result { get; init; }
    
    /// <summary>
    /// Flag that determines if operation was executed successful
    /// </summary>
    public required bool IsSuccessful { get; init; }
    
    /// <summary>
    /// Object that contains details about error that happened
    /// </summary>
    public ApplicationError? Error { get; init; }
}

/// <summary>
/// Object that contains details about error that occured
/// </summary>
public record ApplicationError
{
    /// <summary>
    /// Type of error that happened
    /// </summary>
    public ApplicationErrorType Type { get; init; }
    
    /// <summary>
    /// Additional details about error that happened
    /// </summary>
    public string? Message { get; init; }
}

/// <summary>
/// List of possible error types that occured in application
/// </summary>
public enum ApplicationErrorType
{
    NotFound,
    UnprocessableEntity
}
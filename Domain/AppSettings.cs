using FluentValidation;

namespace Domain;

/// <summary>
/// Object containing all configuration from appsettings file
/// </summary>
public record AppSettings
{
    /// <summary>
    /// Object containing connection details for Boohma api client
    /// </summary>
    public BoohmaServiceSettings BoohmaServiceSettings { get; init; }
    
    /// <summary>
    /// Object containing connection details for postgres database
    /// </summary>
    public PostgresServerSettings PostgresServerSettings { get; init; }
    
    /// <summary>
    /// Number of results counted as latest results
    /// </summary>
    public int LatestResultsCount { get; init; }
    

    public class AppSettingsValidator : AbstractValidator<AppSettings>
    {
        public AppSettingsValidator()
        {
            RuleFor(x => x.LatestResultsCount).NotEmpty();
            RuleFor(x => x.BoohmaServiceSettings).SetValidator(new BoohmaServiceSettings.BoohmaServiceSettingsValidator());
            RuleFor(x => x.PostgresServerSettings).SetValidator(new PostgresServerSettings.PostgresServerSettingsValidator());
        }
    }
}

/// <summary>
/// Object containing connection details for Boohma api client
/// </summary>
public record BoohmaServiceSettings
{
    /// <summary>
    /// Api url
    /// </summary>
    public Uri BaseUrl { get; init; }
    
    public class BoohmaServiceSettingsValidator : AbstractValidator<BoohmaServiceSettings>
    {
        public BoohmaServiceSettingsValidator()
        {
            RuleFor(x => x.BaseUrl).NotEmpty();
        }
    }
}

/// <summary>
/// Object containing connection details for postgres database
/// </summary>
public record PostgresServerSettings
{
    /// <summary>
    /// Database connection string
    /// </summary>
    public string ConnectionString { get; init; }
    
    public class PostgresServerSettingsValidator : AbstractValidator<PostgresServerSettings>
    {
        public PostgresServerSettingsValidator()
        {
            RuleFor(x => x.ConnectionString).NotEmpty();
        }
    }
}
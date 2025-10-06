using Domain;
using FluentValidation;

namespace Api.Configuration;

public static class ConfigurationExtensions
{
    public static AppSettings InitializeAndValidateAppSettings(this WebApplicationBuilder builder)
    {
        var appSettings = builder.Configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();
        if (appSettings == null)
            throw new ArgumentNullException(nameof(AppSettings), "AppSettings configuration section is missing.");
        
        AppSettings.AppSettingsValidator validator = new();
        validator.ValidateAndThrow(appSettings);
        
        return appSettings;
    }
}
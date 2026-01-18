using AvailabilityCompass.Core.Features.ManageSettings;
using AvailabilityCompass.WpfClient.Shared.Theme;
using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.WpfClient.Application.DependencyInjection;

/// <summary>
/// Extension methods for registering settings-related services in the DI container.
/// </summary>
public static class SettingsExtensions
{
    /// <summary>
    /// Adds settings feature services including theme service.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSettings(this IServiceCollection services)
    {
        services.AddSingleton<IThemeService, ThemeService>();
        return services;
    }
}
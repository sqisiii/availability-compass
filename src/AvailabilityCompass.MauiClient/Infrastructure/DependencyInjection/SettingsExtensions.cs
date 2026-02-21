using AvailabilityCompass.Core.Features.ManageSettings;
using AvailabilityCompass.MauiClient.Shared.Theme;

namespace AvailabilityCompass.MauiClient.Infrastructure.DependencyInjection;

public static class SettingsExtensions
{
    public static IServiceCollection AddSettings(this IServiceCollection services)
    {
        services.AddSingleton<IThemeService, MauiThemeService>();
        return services;
    }
}
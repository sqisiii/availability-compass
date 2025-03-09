using AvailabilityCompass.Core.Features.ManageSettings;
using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.WpfClient.Application.DependencyInjection;

public static class SettingsExtensions
{
    public static IServiceCollection AddSettings(this IServiceCollection services)
    {
        services.AddSingleton<ManageSettingsViewModel>();
        return services;
    }
}
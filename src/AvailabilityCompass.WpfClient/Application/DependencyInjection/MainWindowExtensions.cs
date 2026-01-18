using AvailabilityCompass.WpfClient.Application.Initialization;
using AvailabilityCompass.WpfClient.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.WpfClient.Application.DependencyInjection;

/// <summary>
/// Extension methods for registering main window services in the DI container.
/// </summary>
public static class MainWindowExtensions
{
    /// <summary>
    /// Adds main window and bootstrapper services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddMainWindow(this IServiceCollection services)
    {
        services.AddSingleton<Bootstrapper>();
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<MainWindow>();
        return services;
    }
}
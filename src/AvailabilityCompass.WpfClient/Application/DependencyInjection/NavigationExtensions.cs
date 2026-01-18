using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;
using AvailabilityCompass.WpfClient.Shared.Navigation;
using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.WpfClient.Application.DependencyInjection;

/// <summary>
/// Extension methods for registering navigation services in the DI container.
/// </summary>
public static class NavigationExtensions
{
    /// <summary>
    /// Adds navigation services for pages and dialogs.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddNavigation(this IServiceCollection services)
    {
        services.AddSingleton<INavigationTabFactory, NavigationTabFactory>();
        services.AddSingleton<INavigationService<IPageViewModel>, NavigationService>();
        services.AddSingleton<INavigationStore<IPageViewModel>, NavigationStore>();
        services.AddSingleton<INavigationService<IDialogViewModel>, DialogNavigationService>();
        services.AddSingleton<INavigationStore<IDialogViewModel>, DialogNavigationStore>();
        return services;
    }
}
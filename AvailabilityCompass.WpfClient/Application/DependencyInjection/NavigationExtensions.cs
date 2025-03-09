using AvailabilityCompass.WpfClient.Navigation;
using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.WpfClient.Application.DependencyInjection;

public static class NavigationExtensions
{
    public static IServiceCollection AddNavigation(this IServiceCollection services)
    {
        services.AddSingleton<INavigationTabFactory, NavigationTabFactory>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<INavigationStore, NavigationStore>();
        return services;
    }
}
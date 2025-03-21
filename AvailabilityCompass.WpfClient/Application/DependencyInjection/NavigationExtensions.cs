using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;
using AvailabilityCompass.WpfClient.Shared.Navigation;
using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.WpfClient.Application.DependencyInjection;

public static class NavigationExtensions
{
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
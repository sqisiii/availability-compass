using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.Navigation;
using AvailabilityCompass.MauiClient.Shared.Navigation;

namespace AvailabilityCompass.MauiClient.Infrastructure.DependencyInjection;

public static class NavigationExtensions
{
    public static IServiceCollection AddNavigation(this IServiceCollection services)
    {
        services.AddSingleton<INavigationService<IPageViewModel>, NavigationService>();
        services.AddSingleton<INavigationStore<IPageViewModel>, NavigationStore>();
        services.AddSingleton<INavigationService<IDialogViewModel>, DialogNavigationService>();
        services.AddSingleton<INavigationStore<IDialogViewModel>, DialogNavigationStore>();
        return services;
    }
}
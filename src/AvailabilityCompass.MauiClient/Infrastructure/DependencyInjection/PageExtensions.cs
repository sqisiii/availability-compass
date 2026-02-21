using AvailabilityCompass.MauiClient.Pages;
using AvailabilityCompass.MauiClient.Pages.ManageCalendars;
using AvailabilityCompass.MauiClient.Pages.ManageSources;
using AvailabilityCompass.MauiClient.Pages.SearchRecords;

namespace AvailabilityCompass.MauiClient.Infrastructure.DependencyInjection;

public static class PageExtensions
{
    public static IServiceCollection AddPages(this IServiceCollection services)
    {
        services.AddSingleton<AppViewModel>();

        services.AddTransient<SearchPage>();
        services.AddTransient<ManageSourcesPage>();
        services.AddTransient<ManageCalendarsPage>();

        return services;
    }
}
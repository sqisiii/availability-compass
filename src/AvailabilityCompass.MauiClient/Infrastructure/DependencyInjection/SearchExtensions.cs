using AvailabilityCompass.Core.Features.SearchRecords;
using AvailabilityCompass.Core.Features.SearchRecords.FilterFormElements;
using AvailabilityCompass.Core.Features.SearchRecords.Search;
using AvailabilityCompass.Core.Shared;

namespace AvailabilityCompass.MauiClient.Infrastructure.DependencyInjection;

public static class SearchExtensions
{
    public static IServiceCollection AddSearch(this IServiceCollection services)
    {
        services.AddSingleton<SearchViewModel>();
        services.AddSingleton<IFormElementFactory, FormElementFactory>();
        services.AddSingleton<ISearchCommandFactory, SearchCommandFactory>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddTransient<Func<SearchViewModel>>(sp => sp.GetRequiredService<SearchViewModel>);
        return services;
    }
}
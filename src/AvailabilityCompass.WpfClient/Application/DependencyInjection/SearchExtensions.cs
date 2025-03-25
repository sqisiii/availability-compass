using AvailabilityCompass.Core.Features.SearchRecords;
using AvailabilityCompass.Core.Features.SearchRecords.FilterFormElements;
using AvailabilityCompass.Core.Features.SearchRecords.Search;
using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.WpfClient.Application.DependencyInjection;

public static class SearchExtensions
{
    public static IServiceCollection AddSearch(this IServiceCollection services)
    {
        services.AddSingleton<SearchViewModel>();
        services.AddSingleton<IFormElementFactory, FormElementFactory>();
        services.AddSingleton<ISearchCommandFactory, SearchCommandFactory>();
        services.AddTransient<Func<SearchViewModel>>(sp => sp.GetRequiredService<SearchViewModel>); // to break circular dependency
        return services;
    }
}
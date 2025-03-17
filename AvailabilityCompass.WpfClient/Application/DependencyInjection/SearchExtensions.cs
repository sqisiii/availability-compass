using AvailabilityCompass.Core.Features.Search;
using AvailabilityCompass.Core.Features.Search.FilterFormElements;
using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.WpfClient.Application.DependencyInjection;

public static class SearchExtensions
{
    public static IServiceCollection AddSearch(this IServiceCollection services)
    {
        services.AddSingleton<SearchViewModel>();
        services.AddSingleton<IFormElementFactory, FormElementFactory>();
        return services;
    }
}
using AvailabilityCompass.Core.Features.ManageSources;
using AvailabilityCompass.Core.Features.ManageSources.Sources;
using AvailabilityCompass.Core.Features.SearchRecords;
using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.WpfClient.Application.DependencyInjection;

/// <summary>
/// Extension methods for registering source-related services in the DI container.
/// </summary>
public static class SourceExtensions
{
    /// <summary>
    /// Adds source management services including service factory and view model factory.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSource(this IServiceCollection services)
    {
        services.AddSingleton<ManageSourcesViewModel>();
        services.AddSingleton<SourceServiceScanner>();
        services.AddSingleton<ISourceServiceFactory, SourceServiceFactory>();
        services.AddSingleton<ISourceFilterViewModelFactory, SourceFilterViewModelFactory>();
        services.AddSingleton<ISourceMetaDataViewModelFactory, SourceMetaDataViewModelFactory>();
        services.AddSingleton<ISourceStore, SourceStore>();

        return services;
    }
}
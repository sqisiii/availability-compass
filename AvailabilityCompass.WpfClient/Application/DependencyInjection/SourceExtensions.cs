using AvailabilityCompass.Core.Features.ManageSources;
using AvailabilityCompass.Core.Features.ManageSources.Sources;
using AvailabilityCompass.Core.Features.SearchRecords;
using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.WpfClient.Application.DependencyInjection;

public static class SourceExtensions
{
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
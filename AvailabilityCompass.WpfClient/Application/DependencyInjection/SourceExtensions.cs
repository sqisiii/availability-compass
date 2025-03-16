using AvailabilityCompass.Core.Features.ManageSources;
using AvailabilityCompass.Core.Features.ManageSources.Sources;
using AvailabilityCompass.Core.Features.ManageSources.Sources.Horyzonty;
using AvailabilityCompass.Core.Features.ManageSources.Sources.RowerzystaPodroznik;
using AvailabilityCompass.Core.Features.ManageSources.Sources.ViaVerde;
using AvailabilityCompass.Core.Features.Search;
using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.WpfClient.Application.DependencyInjection;

public static class SourceExtensions
{
    public static IServiceCollection AddSource(this IServiceCollection services)
    {
        services.AddSingleton<ManageSourcesViewModel>();
        services.AddSingleton<SourceServiceScanner>();
        services.AddSingleton<ISourceServiceFactory, SourceServiceFactory>();
        services.AddSingleton<ISourceViewModelFactory, SourceViewModelFactory>();
        services.AddSingleton<ISourceMetaDataViewModelFactory, SourceMetaDataViewModelFactory>();
        services.AddKeyedSingleton<ISourceService, HoryzontyService>(HoryzontyService.SourceId);
        services.AddKeyedScoped<ISourceService, RowerzystaPodroznikService>(RowerzystaPodroznikService.SourceId);
        services.AddKeyedSingleton<ISourceService, ViaVerdeService>(ViaVerdeService.SourceId);
        services.AddSingleton<ISourceStore, SourceStore>();

        return services;
    }
}
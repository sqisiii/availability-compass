using AvailabilityCompass.Core.Features.ManageSources;
using AvailabilityCompass.Core.Features.ManageSources.Integrations;
using AvailabilityCompass.Core.Features.ManageSources.Integrations.Horyzonty;
using AvailabilityCompass.Core.Features.Search;
using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.WpfClient.Application.DependencyInjection;

public static class SourceExtensions
{
    public static IServiceCollection AddSource(this IServiceCollection services)
    {
        services.AddSingleton<ManageSourcesViewModel>();
        services.AddSingleton<IntegrationServiceScanner>();
        services.AddSingleton<IIntegrationServiceFactory, IntegrationServiceFactory>();
        services.AddSingleton<ISourceViewModelFactory, SourceViewModelFactory>();
        services.AddSingleton<ISourceMetaDataViewModelFactory, SourceMetaDataViewModelFactory>();
        services.AddKeyedSingleton<IIntegrationService, HoryzontyService>(HoryzontyService.IntegrationId);
        services.AddSingleton<IIntegrationStore, IntegrationStore>();

        return services;
    }
}
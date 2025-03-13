using AvailabilityCompass.Core.Features.ManageSources;
using AvailabilityCompass.Core.Features.Search;
using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.WpfClient.Application.DependencyInjection;

public static class SourceExtensions
{
    public static IServiceCollection AddSource(this IServiceCollection services)
    {
        services.AddSingleton<ManageSourcesViewModel>();
        services.AddSingleton<ISourceViewModelFactory, SourceViewModelFactory>();
        return services;
    }
}
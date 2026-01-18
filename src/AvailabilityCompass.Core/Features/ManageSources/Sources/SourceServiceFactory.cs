using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.Core.Features.ManageSources.Sources;

/// <summary>
/// Factory for retrieving source service instances by their unique key using the DI container.
/// </summary>
public class SourceServiceFactory : ISourceServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public SourceServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public ISourceService GetService(string key)
    {
        return _serviceProvider.GetKeyedService<ISourceService>(key) ?? throw new InvalidOperationException($"Service '{key}' not found.");
    }
}
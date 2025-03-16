using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.Core.Features.ManageSources.Sources;

public class SourceServiceFactory : ISourceServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public SourceServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ISourceService GetService(string key)
    {
        return _serviceProvider.GetKeyedService<ISourceService>(key) ?? throw new InvalidOperationException($"Service '{key}' not found.");
    }
}
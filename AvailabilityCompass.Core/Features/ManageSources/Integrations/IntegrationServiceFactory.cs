using Microsoft.Extensions.DependencyInjection;

namespace AvailabilityCompass.Core.Features.ManageSources.Integrations;

public class IntegrationServiceFactory : IIntegrationServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public IntegrationServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IIntegrationService GetService(string key)
    {
        return _serviceProvider.GetKeyedService<IIntegrationService>(key) ?? throw new InvalidOperationException($"Service '{key}' not found.");
    }
}
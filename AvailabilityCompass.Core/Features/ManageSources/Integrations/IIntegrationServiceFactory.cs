namespace AvailabilityCompass.Core.Features.ManageSources.Integrations;

public interface IIntegrationServiceFactory
{
    IIntegrationService GetService(string key);
}
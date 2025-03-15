namespace AvailabilityCompass.Core.Features.ManageSources.Integrations;

public interface IIntegrationStore
{
    Dictionary<string, string> GetIntegrationsIdAndNames();
}
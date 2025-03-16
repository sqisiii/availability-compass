namespace AvailabilityCompass.Core.Features.ManageSources.Integrations;

public interface IIntegrationStore
{
    IList<IntegrationData> GetIntegrationsIdAndNames();
}
namespace AvailabilityCompass.Core.Features.ManageSources.Integrations;

public class IntegrationStore : IIntegrationStore
{
    private readonly Lazy<Dictionary<string, string>> _integrationsData = new(IntegrationServiceScanner.ScanIntegrationServices);
    public Dictionary<string, string> GetIntegrationsIdAndNames() => _integrationsData.Value;
}
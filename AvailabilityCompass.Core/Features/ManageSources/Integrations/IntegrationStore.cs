namespace AvailabilityCompass.Core.Features.ManageSources.Integrations;

public class IntegrationStore : IIntegrationStore
{
    private readonly Lazy<IList<IntegrationData>> _integrationsData = new(IntegrationServiceScanner.ScanIntegrationServices);
    public IList<IntegrationData> GetIntegrationsIdAndNames() => _integrationsData.Value;
}
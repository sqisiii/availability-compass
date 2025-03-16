namespace AvailabilityCompass.Core.Features.ManageSources.Integrations.ViaVerde;

public class ViaVerdeService : IIntegrationService
{
    //this property is used by Reflection to get the integration id
    public static bool IntegrationEnabled => false;

    //this property is used by Reflection to get the integration name
    public static string IntegrationName => "Via Verde";

    //this property is used by Reflection to get the integration id
    public static string IntegrationId => "ViaVerde";

    public event EventHandler<SourceRefreshProgressEventArgs>? RefreshProgressChanged;

    public Task<IEnumerable<SourceDataItem>> RefreshIntegrationDataAsync(CancellationToken ct)
    {
        return Task.FromResult(Enumerable.Empty<SourceDataItem>());
    }
}
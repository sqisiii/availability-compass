namespace AvailabilityCompass.Core.Features.ManageSources.Integrations.RowerzystaPodroznik;

public class RowerzystaPodroznikService : IIntegrationService
{
    //this property is used by Reflection to get the integration id
    public static bool IntegrationEnabled => true;

    //this property is used by Reflection to get the integration name
    public static string IntegrationName => "Rowerzysta Podróżnik";

    //this property is used by Reflection to get the integration id
    public static string IntegrationId => "RowerzystaPodroznik";

    public event EventHandler<SourceRefreshProgressEventArgs>? RefreshProgressChanged;

    public Task<IEnumerable<SourceDataItem>> RefreshIntegrationDataAsync(CancellationToken ct)
    {
        return Task.FromResult(Enumerable.Empty<SourceDataItem>());
    }
}
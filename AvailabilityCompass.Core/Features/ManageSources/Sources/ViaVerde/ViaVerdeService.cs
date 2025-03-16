namespace AvailabilityCompass.Core.Features.ManageSources.Sources.ViaVerde;

public class ViaVerdeService : ISourceService
{
    //this property is used by Reflection to get the source id
    public static bool SourceEnabled => false;

    //this property is used by Reflection to get the source name
    public static string SourceName => "Via Verde";

    //this property is used by Reflection to get the source id
    public static string SourceId => "ViaVerde";

    public event EventHandler<SourceRefreshProgressEventArgs>? RefreshProgressChanged;

    public Task<IEnumerable<SourceDataItem>> RefreshSourceDataAsync(CancellationToken ct)
    {
        return Task.FromResult(Enumerable.Empty<SourceDataItem>());
    }
}
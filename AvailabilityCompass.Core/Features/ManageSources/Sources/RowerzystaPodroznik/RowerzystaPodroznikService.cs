namespace AvailabilityCompass.Core.Features.ManageSources.Sources.RowerzystaPodroznik;

public class RowerzystaPodroznikService : ISourceService
{
    //this property is used by Reflection to get the source id
    public static bool SourceEnabled => true;

    //this property is used by Reflection to get the source name
    public static string SourceName => "Rowerzysta Podróżnik";

    //this property is used by Reflection to get the source id
    public static string SourceId => "RowerzystaPodroznik";

    public event EventHandler<SourceRefreshProgressEventArgs>? RefreshProgressChanged;

    public Task<IEnumerable<SourceDataItem>> RefreshSourceDataAsync(CancellationToken ct)
    {
        return Task.FromResult(Enumerable.Empty<SourceDataItem>());
    }
}
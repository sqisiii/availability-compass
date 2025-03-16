namespace AvailabilityCompass.Core.Features.ManageSources.Sources.RowerzystaPodroznik;

[SourceService("RowerzystaPodroznik", "Rowerzysta Podróżnik")]
public class RowerzystaPodroznikService : ISourceService
{
    public event EventHandler<SourceRefreshProgressEventArgs>? RefreshProgressChanged;

    public Task<IEnumerable<SourceDataItem>> RefreshSourceDataAsync(CancellationToken ct)
    {
        return Task.FromResult(Enumerable.Empty<SourceDataItem>());
    }
}
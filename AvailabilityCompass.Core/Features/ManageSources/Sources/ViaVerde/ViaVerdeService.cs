namespace AvailabilityCompass.Core.Features.ManageSources.Sources.ViaVerde;

[SourceService("ViaVerde", "Via Verde", false)]
public class ViaVerdeService : ISourceService
{
    public event EventHandler<SourceRefreshProgressEventArgs>? RefreshProgressChanged;

    public Task<IEnumerable<SourceDataItem>> RefreshSourceDataAsync(CancellationToken ct)
    {
        return Task.FromResult(Enumerable.Empty<SourceDataItem>());
    }
}
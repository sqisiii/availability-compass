namespace AvailabilityCompass.Core.Features.ManageSources.Sources.ViaVerde;

[SourceService("ViaVerde", "Via Verde", "PL", false)]
public class ViaVerdeService : ISourceService
{
    public event EventHandler<SourceRefreshProgressEventArgs>? RefreshProgressChanged;

    public Task<IEnumerable<SourceDataItem>> RefreshSourceDataAsync(CancellationToken ct)
    {
        return Task.FromResult(Enumerable.Empty<SourceDataItem>());
    }

    public Task<List<SourceFilter>> GetFilters(CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
namespace AvailabilityCompass.Core.Features.ManageSources.Sources;

public interface ISourceService
{
    event EventHandler<SourceRefreshProgressEventArgs> RefreshProgressChanged;

    Task<IEnumerable<SourceDataItem>> RefreshSourceDataAsync(CancellationToken ct);

    Task<List<SourceFilter>> GetFilters(CancellationToken ct);
}
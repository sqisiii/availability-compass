using AvailabilityCompass.Core.Features.ManageSources.Commands.ReplaceSourceDataRequest;
using AvailabilityCompass.Core.Features.ManageSources.Queries.GetFilterOptionsQuery;
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageSources.Sources;

/// <summary>
/// Abstract base class for source services.
/// Provides common functionality for data refresh, progress reporting, and filter retrieval.
/// </summary>
public abstract class SourceServiceBase : ISourceService
{
    protected readonly HttpClient HttpClient;
    private readonly IMediator Mediator;
    protected readonly string SourceId;

    protected SourceServiceBase(HttpClient httpClient, IMediator mediator)
    {
        HttpClient = httpClient;
        Mediator = mediator;
        SourceId = this.GetSourceId() ?? string.Empty;
    }

    /// <summary>
    /// Defines the filter field names for this source.
    /// Used by <see cref="GetFilters"/> to query available options.
    /// </summary>
    protected abstract IReadOnlyList<string> FilterFieldNames { get; }

    public event EventHandler<SourceRefreshProgressEventArgs>? RefreshProgressChanged;

    /// <summary>
    /// Template Method: Refreshes source data by extracting trips and persisting them.
    /// The extraction logic is delegated to derived classes via <see cref="ExtractSourceDataAsync"/>.
    /// </summary>
    public async Task<IEnumerable<SourceDataItem>> RefreshSourceDataAsync(CancellationToken ct)
    {
        var trips = await ExtractSourceDataAsync(ct);
        await Mediator.Send(new ReplaceSourceDataInDbRequest(trips), ct);
        return trips;
    }

    /// <summary>
    /// Retrieves available filters for this source based on <see cref="FilterFieldNames"/>.
    /// </summary>
    public virtual async Task<List<SourceFilter>> GetFilters(CancellationToken ct)
    {
        var options = await Mediator.Send(new GetFilterOptionsQuery(SourceId, FilterFieldNames.ToList()), ct);
        return FilterFieldNames.Select(field => new SourceFilter
            {
                Label = field,
                Type = SourceFilterType.MultiSelect,
                Options = options.FilterOptions.GetValueOrDefault(field, [])
            })
            .ToList();
    }

    /// <summary>
    /// Abstract method for extracting source data.
    /// </summary>
    protected abstract Task<IReadOnlyCollection<SourceDataItem>> ExtractSourceDataAsync(CancellationToken ct);

    /// <summary>
    /// Reports progress during data extraction.
    /// </summary>
    /// <param name="progressPercentage">Progress percentage (0-100).</param>
    protected void ReportProgress(double progressPercentage)
    {
        RefreshProgressChanged?.Invoke(this, new SourceRefreshProgressEventArgs(SourceId, progressPercentage));
    }
}
namespace AvailabilityCompass.Core.Features.ManageSources.Sources;

/// <summary>
/// Interface for source service operations.
/// </summary>
public interface ISourceService
{
    /// <summary>
    /// Event triggered when the refresh progress changes.
    /// </summary>
    event EventHandler<SourceRefreshProgressEventArgs> RefreshProgressChanged;

    /// <summary>
    /// Asynchronously refreshes the source data.
    /// </summary>
    /// <param name="ct">Cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of <see cref="SourceDataItem"/>.</returns>
    Task<IEnumerable<SourceDataItem>> RefreshSourceDataAsync(CancellationToken ct);

    /// <summary>
    /// Asynchronously retrieves the list of source filters.
    /// </summary>
    /// <param name="ct">Cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="SourceFilter"/>.</returns>
    Task<List<SourceFilter>> GetFilters(CancellationToken ct);
}

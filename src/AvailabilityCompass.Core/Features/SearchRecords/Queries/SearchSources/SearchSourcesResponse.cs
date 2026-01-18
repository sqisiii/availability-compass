namespace AvailabilityCompass.Core.Features.SearchRecords.Queries.SearchSources;

/// <summary>
/// Response containing search results from queried sources.
/// </summary>
public class SearchSourcesResponse
{
    public SearchSourcesResponse(IReadOnlyCollection<SourceDataItem> sourceDataItems)
    {
        SourceDataItems = sourceDataItems;
    }

    public IReadOnlyCollection<SourceDataItem> SourceDataItems { get; }

    public bool IsSuccess { get; init; } = true;

    /// <summary>
    /// Represents a single search result item from a source.
    /// </summary>
    public record SourceDataItem(
        int SeqNo,
        string SourceId,
        string? Title,
        string? Url,
        DateOnly StartDate,
        DateOnly EndDate,
        DateTime ChangeDate,
        Dictionary<string, object?> AdditionalData)
    {
        //required for Dapper
        public SourceDataItem() : this(0, string.Empty, string.Empty, string.Empty, new DateOnly(), new DateOnly(), DateTime.MinValue, new Dictionary<string, object?>())
        {
        }
    }
}
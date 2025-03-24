namespace AvailabilityCompass.Core.Features.SearchRecords.Queries.SearchSources;

public class SearchSourcesResponse
{
    public SearchSourcesResponse(IReadOnlyCollection<SourceDataItem> sourceDataItems)
    {
        SourceDataItems = sourceDataItems;
    }

    public IReadOnlyCollection<SourceDataItem> SourceDataItems { get; }

    public bool IsSuccess { get; init; } = true;

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
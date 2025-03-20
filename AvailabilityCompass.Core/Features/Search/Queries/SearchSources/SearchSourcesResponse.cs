using AvailabilityCompass.Core.Features.ManageSources;

namespace AvailabilityCompass.Core.Features.Search.Queries.SearchSources;

public class SearchSourcesResponse
{
    public SearchSourcesResponse(IReadOnlyCollection<SourceDataItem> sourceDataItems)
    {
        SourceDataItems = sourceDataItems;
    }

    public IReadOnlyCollection<SourceDataItem> SourceDataItems { get; }

    public bool IsSuccess { get; init; } = true;
}
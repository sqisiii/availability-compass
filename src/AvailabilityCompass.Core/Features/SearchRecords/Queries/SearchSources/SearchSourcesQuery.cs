using MediatR;

namespace AvailabilityCompass.Core.Features.SearchRecords.Queries.SearchSources;

/// <summary>
/// MediatR query to search sources with the specified filters and reserved dates.
/// </summary>
public class SearchSourcesQuery : IRequest<SearchSourcesResponse>
{
    public List<DateOnly> ReservedDates { get; init; } = [];

    public List<Source> Sources { get; } = [];

    /// <summary>
    /// Represents a source with selected filter values for the search query.
    /// </summary>
    public class Source
    {
        public Source(string sourceId)
        {
            SourceId = sourceId;
        }

        public string SourceId { get; }

        public Dictionary<string, List<string>> SelectedFiltersValues { get; } = new();
    }
}
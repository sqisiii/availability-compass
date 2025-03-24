using MediatR;

namespace AvailabilityCompass.Core.Features.SearchRecords.Queries.SearchSources;

public class SearchSourcesQuery : IRequest<SearchSourcesResponse>
{
    public List<DateOnly> ReservedDates { get; init; } = [];

    public List<Source> Sources { get; } = [];

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
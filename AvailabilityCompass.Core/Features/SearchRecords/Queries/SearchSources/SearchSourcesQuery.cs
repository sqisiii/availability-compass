using MediatR;

namespace AvailabilityCompass.Core.Features.SearchRecords.Queries.SearchSources;

public class SearchSourcesQuery : IRequest<SearchSourcesResponse>
{
    public int PageNumber { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    public List<DateOnly> ReservedDates { get; set; } = [];

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
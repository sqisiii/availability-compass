namespace AvailabilityCompass.Core.Features.Search.Queries.GetSourcesQuery;

public class GetSourcesResponse
{
    public List<Source> Sources { get; set; } = [];

    public IReadOnlyDictionary<string, IReadOnlyCollection<string>> AvailableFilters { get; } = new Dictionary<string, IReadOnlyCollection<string>>();

    public class Source
    {
        public DateTime ChangedAt { get; set; }
        public bool IsEnabled { get; set; } = true;
        public string Name { get; set; } = string.Empty;
    }
}
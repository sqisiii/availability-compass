namespace AvailabilityCompass.Core.Features.Search.Queries.GetSources;

public class GetSourcesForFilteringResponse
{
    public List<Source> Sources { get; set; } = [];

    public IReadOnlyDictionary<string, IReadOnlyCollection<string>> AvailableFilters { get; } = new Dictionary<string, IReadOnlyCollection<string>>();

    public class Source
    {
        public DateTime? ChangedAt { get; init; }
        public bool IsEnabled { get; init; } = true;
        public string Name { get; init; } = string.Empty;
    }
}
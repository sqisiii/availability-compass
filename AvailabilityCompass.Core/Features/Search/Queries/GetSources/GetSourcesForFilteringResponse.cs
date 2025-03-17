namespace AvailabilityCompass.Core.Features.Search.Queries.GetSources;

public class GetSourcesForFilteringResponse
{
    public enum SourceFilterType
    {
        Text,
        Boolean,
        MultiSelect
    }

    public List<Source> Sources { get; set; } = [];

    public IReadOnlyDictionary<string, IReadOnlyCollection<string>> AvailableFilters { get; } = new Dictionary<string, IReadOnlyCollection<string>>();

    public class Source
    {
        public string SourceId { get; set; } = string.Empty;
        public DateTime? ChangedAt { get; init; }
        public bool IsEnabled { get; init; } = true;
        public string Name { get; init; } = string.Empty;

        public List<SourceFilter> Filters { get; set; } = [];
    }

    public class SourceFilter
    {
        public string Label { get; set; } = string.Empty;

        public SourceFilterType Type { get; set; }

        public List<string> Options { get; set; } = new();
    }
}
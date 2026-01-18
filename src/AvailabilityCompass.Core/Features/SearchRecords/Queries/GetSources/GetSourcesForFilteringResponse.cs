namespace AvailabilityCompass.Core.Features.SearchRecords.Queries.GetSources;

/// <summary>
/// Response containing sources available for filtering in search with their filter configurations.
/// </summary>
public class GetSourcesForFilteringResponse
{
    /// <summary>
    /// Defines the types of filters available for sources.
    /// </summary>
    public enum SourceFilterType
    {
        Text,
        Boolean,
        MultiSelect
    }

    public List<Source> Sources { get; } = [];

    public IReadOnlyDictionary<string, IReadOnlyCollection<string>> AvailableFilters { get; } = new Dictionary<string, IReadOnlyCollection<string>>();

    /// <summary>
    /// Represents a source with its metadata and filter configuration.
    /// </summary>
    public class Source
    {
        public string SourceId { get; init; } = string.Empty;
        public DateTime? ChangedAt { get; init; }
        public bool IsEnabled { get; init; } = true;
        public string Name { get; init; } = string.Empty;
        public string Language { get; init; } = string.Empty;
        public string IconFileName { get; init; } = string.Empty;

        public List<SourceFilter> Filters { get; init; } = [];
    }

    /// <summary>
    /// Represents a filter configuration for a source.
    /// </summary>
    public class SourceFilter
    {
        public string Label { get; set; } = string.Empty;

        public SourceFilterType Type { get; set; }

        public List<string> Options { get; set; } = new();
    }
}
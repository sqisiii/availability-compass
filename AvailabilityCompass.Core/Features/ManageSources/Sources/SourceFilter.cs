namespace AvailabilityCompass.Core.Features.ManageSources.Sources;

/// <summary>
/// Represents a filter supported by source services
/// </summary>
public class SourceFilter
{
    public string Label { get; init; } = string.Empty;

    public SourceFilterType Type { get; init; }

    public List<string> Options { get; init; } = [];
}
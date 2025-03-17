namespace AvailabilityCompass.Core.Features.ManageSources.Sources;

public class SourceFilter
{
    public string Label { get; set; } = string.Empty;

    public SourceFilterType Type { get; set; }

    public List<string> Options { get; set; } = new();
}
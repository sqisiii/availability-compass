namespace AvailabilityCompass.Core.Features.ManageSources;

public class SourceDefinition
{
    public string SourceId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime? ChangedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public List<SourceFilter> Filters { get; set; } = new();
}
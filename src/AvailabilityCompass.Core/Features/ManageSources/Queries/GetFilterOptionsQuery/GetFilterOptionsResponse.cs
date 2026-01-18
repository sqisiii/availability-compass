namespace AvailabilityCompass.Core.Features.ManageSources.Queries.GetFilterOptionsQuery;

/// <summary>
/// Response containing filter options for a source.
/// </summary>
public class GetFilterOptionsResponse
{
    public Dictionary<string, List<string>> FilterOptions { get; set; } = new();
}
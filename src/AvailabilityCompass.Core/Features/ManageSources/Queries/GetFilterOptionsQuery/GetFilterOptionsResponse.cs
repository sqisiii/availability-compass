namespace AvailabilityCompass.Core.Features.ManageSources.Queries.GetFilterOptionsQuery;

public class GetFilterOptionsResponse
{
    public Dictionary<string, List<string>> FilterOptions { get; set; } = new();
}
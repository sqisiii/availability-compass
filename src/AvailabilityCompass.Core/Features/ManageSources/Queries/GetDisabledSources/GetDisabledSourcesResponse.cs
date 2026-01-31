namespace AvailabilityCompass.Core.Features.ManageSources.Queries.GetDisabledSources;

/// <summary>
/// Response containing the set of disabled source IDs.
/// </summary>
public class GetDisabledSourcesResponse
{
    public GetDisabledSourcesResponse(HashSet<string> disabledSourceIds)
    {
        DisabledSourceIds = disabledSourceIds;
    }

    public HashSet<string> DisabledSourceIds { get; }
}

namespace AvailabilityCompass.Core.Features.ManageSources.Queries.GetSourcesMetaDataFromDbQuery;

public class GetSourcesMetaDataFromDbDto
{
    public string SourceId { get; init; } = string.Empty;

    public DateTime? ChangedAt { get; init; }

    public int TripsCount { get; init; }
}
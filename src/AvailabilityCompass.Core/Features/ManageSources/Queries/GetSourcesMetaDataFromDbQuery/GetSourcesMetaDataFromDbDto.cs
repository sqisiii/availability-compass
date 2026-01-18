namespace AvailabilityCompass.Core.Features.ManageSources.Queries.GetSourcesMetaDataFromDbQuery;

/// <summary>
/// Data transfer object containing source metadata from the database.
/// </summary>
public class GetSourcesMetaDataFromDbDto
{
    public string SourceId { get; init; } = string.Empty;

    public DateTime? ChangedAt { get; init; }

    public int TripsCount { get; init; }
}
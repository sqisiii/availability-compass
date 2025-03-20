namespace AvailabilityCompass.Core.Features.ManageSources;

public record SourceDataItem(
    int SeqNo,
    string SourceId,
    string? Title,
    string? Url,
    DateOnly StartDate,
    DateOnly EndDate,
    DateTime ChangeDate,
    Dictionary<string, object?> AdditionalData)
{
    public SourceDataItem() : this(0, string.Empty, null, null, new DateOnly(), new DateOnly(), DateTime.MinValue, new Dictionary<string, object?>())
    {
    }
}
namespace AvailabilityCompass.Core.Features.ManageSources;

public record SourceDataItem(
    int SeqNo,
    string SourceId,
    string? Title,
    DateOnly StartDate,
    DateOnly EndDate,
    DateTime ChangeDate,
    Dictionary<string, object?> AdditionalData);
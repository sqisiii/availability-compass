namespace AvailabilityCompass.Core.Features.ManageSources;

public record SourceDataItem(
    int SeqNo,
    string IntegrationId,
    string? Title,
    string? Type,
    string? Country,
    DateOnly StartDate,
    DateOnly EndDate,
    double? Price,
    DateTime ChangeDate,
    Dictionary<string, object?> AdditionalData);
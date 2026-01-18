namespace AvailabilityCompass.Core.Features.ManageSources.Commands.ReplaceSourceDataRequest;

/// <summary>
/// Response from the replace source data command indicating success or failure.
/// </summary>
public record ReplaceSourceDataInDbResponse(bool IsSuccess);
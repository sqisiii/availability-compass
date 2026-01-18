namespace AvailabilityCompass.Core.Features.SearchRecords.Queries.GetAvailableDates;

/// <summary>
/// Response containing reserved dates calculated from selected calendars.
/// </summary>
public record GetAvailableDatesResponse(bool IsSuccess, List<DateOnly> ReservedDates);
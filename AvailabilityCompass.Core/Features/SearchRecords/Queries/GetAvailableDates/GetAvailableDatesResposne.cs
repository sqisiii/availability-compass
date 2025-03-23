namespace AvailabilityCompass.Core.Features.SearchRecords.Queries.GetAvailableDates;

public record GetAvailableDatesResponse(bool IsSuccess, List<DateOnly> ReservedDates);
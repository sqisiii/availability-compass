namespace AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetCalendarsWithEntries;

/// <summary>
/// Response for calendars with their date entries.
/// </summary>
public record GetCalendarsWithEntriesResponse(bool IsSuccess, List<CalendarDto> Calendars);
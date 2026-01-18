namespace AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetCalendarsQuery;

/// <summary>
/// Response containing the list of calendars with their date entries.
/// </summary>
public class GetCalendarsResponse
{
    public GetCalendarsResponse(List<CalendarDto>? calendars, bool isSuccess)
    {
        Calendars = calendars;
        IsSuccess = isSuccess;
    }

    public bool IsSuccess { get; init; }
    public List<CalendarDto>? Calendars { get; }
}
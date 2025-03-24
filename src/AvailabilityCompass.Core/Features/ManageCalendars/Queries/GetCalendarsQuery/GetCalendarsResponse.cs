namespace AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetCalendarsQuery;

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
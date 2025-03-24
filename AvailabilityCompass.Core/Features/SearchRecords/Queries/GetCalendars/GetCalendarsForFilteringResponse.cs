namespace AvailabilityCompass.Core.Features.SearchRecords.Queries.GetCalendars;

public class GetCalendarsForFilteringResponse
{
    public bool IsSuccess;

    public GetCalendarsForFilteringResponse(List<CalendarDto> calendars, bool isSuccess)
    {
        IsSuccess = isSuccess;
        Calendars = calendars;
    }

    public List<CalendarDto> Calendars { get; }

    public class CalendarDto
    {
        public string Name { get; init; } = string.Empty;
        public Guid Id { get; init; } = Guid.Empty;
        public bool IsOnly { get; init; }
    }
}
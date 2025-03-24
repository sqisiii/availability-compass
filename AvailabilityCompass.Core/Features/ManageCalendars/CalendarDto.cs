namespace AvailabilityCompass.Core.Features.ManageCalendars;

public class CalendarDto
{
    //required for Dapper
    public CalendarDto()
    {
    }

    public CalendarDto(Guid calendarId, string name, bool isOnly, DateTime changeDate)
    {
        CalendarId = calendarId;
        Name = name;
        IsOnly = isOnly;
        ChangeDate = changeDate;
    }

    public Guid CalendarId { get; }

    public string Name { get; } = string.Empty;

    public bool IsOnly { get; }

    public DateTime ChangeDate { get; }

    public List<SingleDateDto> SingleDates { get; } = [];

    public List<RecurringDateDto> RecurringDates { get; } = [];
}
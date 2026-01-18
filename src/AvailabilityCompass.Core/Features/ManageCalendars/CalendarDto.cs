namespace AvailabilityCompass.Core.Features.ManageCalendars;

/// <summary>
/// Data transfer object representing a calendar with its associated date entries.
/// </summary>
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

    public List<DateEntryDto> DateEntries { get; } = [];
}
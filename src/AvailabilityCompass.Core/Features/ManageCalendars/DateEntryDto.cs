namespace AvailabilityCompass.Core.Features.ManageCalendars;

/// <summary>
/// Data transfer object representing a date entry within a calendar.
/// </summary>
public record DateEntryDto(
    Guid CalendarId,
    Guid DateEntryId,
    DateOnly StartDate,
    string Description,
    bool IsRecurring,
    int Duration,
    int? Frequency,
    int NumberOfRepetitions,
    DateTime ChangeDate)
{
    public DateEntryDto() : this(
        Guid.Empty,
        Guid.Empty,
        DateOnly.MinValue,
        string.Empty,
        false,
        1,
        null,
        0,
        DateTime.MinValue)
    {
    }
}

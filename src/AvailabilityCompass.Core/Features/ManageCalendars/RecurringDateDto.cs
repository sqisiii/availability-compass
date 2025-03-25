namespace AvailabilityCompass.Core.Features.ManageCalendars;

public record RecurringDateDto(
    Guid CalendarId,
    Guid RecurringDateId,
    string RecurringDateDescription,
    DateOnly StartDate,
    int Duration,
    int? Frequency,
    int NumberOfRepetitions,
    DateTime ChangeDate)
{
    public RecurringDateDto() : this(Guid.Empty,
        Guid.Empty,
        string.Empty,
        DateOnly.MinValue,
        1,
        null,
        0,
        DateTime.MinValue)
    {
    }
}
namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteDateEntryRequest;

/// <summary>
/// Event published when a date entry is deleted from a calendar.
/// </summary>
public class DateEntryDeletedEvent
{
    public DateEntryDeletedEvent(Guid calendarId)
    {
        CalendarId = calendarId;
    }

    public Guid CalendarId { get; }
}

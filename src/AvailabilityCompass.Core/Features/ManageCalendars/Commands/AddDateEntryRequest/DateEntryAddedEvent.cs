namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddDateEntryRequest;

/// <summary>
/// Event published when a new date entry is added to a calendar.
/// </summary>
public class DateEntryAddedEvent
{
    public DateEntryAddedEvent(Guid calendarId)
    {
        CalendarId = calendarId;
    }

    public Guid CalendarId { get; }
}

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateDateEntryRequest;

/// <summary>
/// Event published when a date entry is updated in the database.
/// </summary>
public class DateEntryUpdatedEvent
{
    public DateEntryUpdatedEvent(Guid calendarId)
    {
        CalendarId = calendarId;
    }

    public Guid CalendarId { get; }
}

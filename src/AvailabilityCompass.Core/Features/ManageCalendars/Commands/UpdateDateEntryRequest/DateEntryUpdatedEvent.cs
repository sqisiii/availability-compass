namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateDateEntryRequest;

public class DateEntryUpdatedEvent
{
    public DateEntryUpdatedEvent(Guid calendarId)
    {
        CalendarId = calendarId;
    }

    public Guid CalendarId { get; }
}

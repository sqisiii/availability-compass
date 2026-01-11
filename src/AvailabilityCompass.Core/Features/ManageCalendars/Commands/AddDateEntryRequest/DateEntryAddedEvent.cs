namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddDateEntryRequest;

public class DateEntryAddedEvent
{
    public DateEntryAddedEvent(Guid calendarId)
    {
        CalendarId = calendarId;
    }

    public Guid CalendarId { get; }
}

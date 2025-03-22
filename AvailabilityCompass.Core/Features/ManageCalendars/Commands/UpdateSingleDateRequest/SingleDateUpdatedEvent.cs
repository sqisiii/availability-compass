namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateSingleDateRequest;

public class SingleDateUpdatedEvent
{
    public SingleDateUpdatedEvent(Guid calendarId)
    {
        CalendarId = calendarId;
    }

    public Guid CalendarId { get; }
}
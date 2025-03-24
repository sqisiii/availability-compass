namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddSingleDateRequest;

public class SingleDateAddedEvent
{
    public SingleDateAddedEvent(Guid calendarId)
    {
        CalendarId = calendarId;
    }

    public Guid CalendarId { get; }
}
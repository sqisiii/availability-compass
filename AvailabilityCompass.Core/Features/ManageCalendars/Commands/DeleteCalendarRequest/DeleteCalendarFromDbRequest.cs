using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteCalendarRequest;

public class DeleteCalendarFromDbRequest : IRequest<DeleteCalendarFromDbResponse>
{
    public DeleteCalendarFromDbRequest(Guid calendarId)
    {
        CalendarId = calendarId;
    }

    public Guid CalendarId { get; set; }
}
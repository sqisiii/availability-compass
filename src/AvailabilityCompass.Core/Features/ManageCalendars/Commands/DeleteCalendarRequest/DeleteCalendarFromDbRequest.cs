using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteCalendarRequest;

/// <summary>
/// MediatR request to delete a calendar from the database.
/// </summary>
public class DeleteCalendarFromDbRequest : IRequest<DeleteCalendarFromDbResponse>
{
    public DeleteCalendarFromDbRequest(Guid calendarId)
    {
        CalendarId = calendarId;
    }

    public Guid CalendarId { get; }
}
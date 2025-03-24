using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteSingleDateRequest;

public class DeleteSingleDateFromDbRequest : IRequest<DeleteSingleDateFromDbResponse>
{
    public DeleteSingleDateFromDbRequest(Guid calendarId, Guid singleDateId)
    {
        CalendarId = calendarId;
        SingleDateId = singleDateId;
    }

    public Guid CalendarId { get; }
    public Guid SingleDateId { get; }
}
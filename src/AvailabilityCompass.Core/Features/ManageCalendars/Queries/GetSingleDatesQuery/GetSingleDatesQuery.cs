using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetSingleDatesQuery;

public class GetSingleDatesQuery : IRequest<GetSingleDatesResponse>
{
    public GetSingleDatesQuery(Guid calendarId)
    {
        CalendarId = calendarId;
    }

    public Guid CalendarId { get; }
}
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetDateEntriesQuery;

public class GetDateEntriesQuery : IRequest<GetDateEntriesResponse>
{
    public GetDateEntriesQuery(Guid calendarId)
    {
        CalendarId = calendarId;
    }

    public Guid CalendarId { get; }
}

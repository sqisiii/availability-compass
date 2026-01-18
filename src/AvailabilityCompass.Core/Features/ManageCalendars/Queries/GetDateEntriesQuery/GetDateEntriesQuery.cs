using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetDateEntriesQuery;

/// <summary>
/// MediatR query to retrieve date entries for a specific calendar.
/// </summary>
public class GetDateEntriesQuery : IRequest<GetDateEntriesResponse>
{
    public GetDateEntriesQuery(Guid calendarId)
    {
        CalendarId = calendarId;
    }

    public Guid CalendarId { get; }
}

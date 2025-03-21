using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetRecurringDatesQuery;

public class GetRecurringDatesQuery : IRequest<GetRecurringDatesResponse>
{
    public GetRecurringDatesQuery(Guid calendarId)
    {
        CalendarId = calendarId;
    }

    public Guid CalendarId { get; }
}
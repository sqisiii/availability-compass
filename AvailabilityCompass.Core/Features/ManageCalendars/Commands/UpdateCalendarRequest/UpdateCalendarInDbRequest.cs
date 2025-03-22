using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateCalendarRequest;

public class UpdateCalendarInDbRequest : IRequest<UpdateCalendarInDbResponse>
{
    public UpdateCalendarInDbRequest(Guid calendarId, string name, bool isOnly)
    {
        CalendarId = calendarId;
        Name = name;
        IsOnly = isOnly;
    }

    public Guid CalendarId { get; }

    public string Name { get; }

    public bool IsOnly { get; }
}
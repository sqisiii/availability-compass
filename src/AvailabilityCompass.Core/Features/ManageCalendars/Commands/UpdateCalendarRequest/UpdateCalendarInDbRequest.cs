using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateCalendarRequest;

/// <summary>
/// MediatR request to update an existing calendar in the database.
/// </summary>
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
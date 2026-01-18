using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteDateEntryRequest;

/// <summary>
/// MediatR request to delete a date entry from a calendar.
/// </summary>
public class DeleteDateEntryFromDbRequest : IRequest<DeleteDateEntryFromDbResponse>
{
    public DeleteDateEntryFromDbRequest(Guid calendarId, Guid dateEntryId)
    {
        CalendarId = calendarId;
        DateEntryId = dateEntryId;
    }

    public Guid CalendarId { get; }
    public Guid DateEntryId { get; }
}

using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateSingleDateRequest;

public class UpdateSingleDateInDbRequest : IRequest<UpdateSingleDateInDbResponse>
{
    public UpdateSingleDateInDbRequest(Guid calendarId, string description, DateOnly date, Guid singleDateId)
    {
        CalendarId = calendarId;
        Description = description;
        Date = date;
        SingleDateId = singleDateId;
    }

    public Guid CalendarId { get; }
    public string Description { get; }
    public DateOnly Date { get; }
    public Guid SingleDateId { get; }
}
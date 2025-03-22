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

    public Guid CalendarId { get; set; }
    public string Description { get; set; }
    public DateOnly Date { get; set; }
    public Guid SingleDateId { get; set; }
}
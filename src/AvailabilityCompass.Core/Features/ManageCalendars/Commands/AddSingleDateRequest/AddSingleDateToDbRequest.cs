using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddSingleDateRequest;

public class AddSingleDateToDbRequest : IRequest<AddSingleDateToDbResponse>
{
    public AddSingleDateToDbRequest(Guid calendarId, string description, DateOnly date)
    {
        CalendarId = calendarId;
        Description = description;
        Date = date;
    }

    public Guid CalendarId { get; }

    public string Description { get; }

    public DateOnly Date { get; }
}
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateRecurringDateRequest;

public class UpdateRecurringDateInDbRequest : IRequest<UpdateRecurringDateInDbResponse>
{
    public UpdateRecurringDateInDbRequest(Guid calendarId, Guid recurringDateId, DateOnly startDate, int duration, string description, int? frequency, int? numberOfRepetitions)
    {
        CalendarId = calendarId;
        RecurringDateId = recurringDateId;
        StartDate = startDate;
        Duration = duration;
        Description = description;
        Frequency = frequency;
        NumberOfRepetitions = numberOfRepetitions;
    }

    public Guid CalendarId { get; }
    public Guid RecurringDateId { get; }
    public DateOnly StartDate { get; }
    public int Duration { get; }
    public string Description { get; }
    public int? Frequency { get; }
    public int? NumberOfRepetitions { get; }
}
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateRecurringDateRequest;

public class UpdateRecurringDateInDbRequest : IRequest<UpdateRecurringDateInDbResponse>
{
    public UpdateRecurringDateInDbRequest(Guid calendarId, Guid recurringDateId, DateOnly startDate, int duration, string description, int repetitionPeriod, int numberOfRepetitions)
    {
        CalendarId = calendarId;
        RecurringDateId = recurringDateId;
        StartDate = startDate;
        Duration = duration;
        Description = description;
        RepetitionPeriod = repetitionPeriod;
        NumberOfRepetitions = numberOfRepetitions;
    }

    public Guid CalendarId { get; set; }
    public Guid RecurringDateId { get; set; }
    public DateOnly StartDate { get; set; }
    public int Duration { get; set; }
    public string Description { get; set; }
    public int RepetitionPeriod { get; set; }
    public int NumberOfRepetitions { get; set; }
}
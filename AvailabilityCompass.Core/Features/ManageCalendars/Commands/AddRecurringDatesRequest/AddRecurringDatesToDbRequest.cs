using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddRecurringDatesRequest;

public class AddRecurringDatesToDbRequest : IRequest<AddRecurringDatesToDbResponse>
{
    public AddRecurringDatesToDbRequest(Guid calendarId, string description, DateOnly startDate, int duration, int repetitionPeriod, int numberOfRepetitions)
    {
        CalendarId = calendarId;
        Description = description;
        StartDate = startDate;
        Duration = duration;
        RepetitionPeriod = repetitionPeriod;
        NumberOfRepetitions = numberOfRepetitions;
    }

    public Guid CalendarId { get; }

    public string Description { get; }

    public DateOnly StartDate { get; }

    public int Duration { get; }

    public int RepetitionPeriod { get; }

    public int NumberOfRepetitions { get; }
}
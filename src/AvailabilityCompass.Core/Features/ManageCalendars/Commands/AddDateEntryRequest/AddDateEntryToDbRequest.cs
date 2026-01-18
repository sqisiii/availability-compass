using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddDateEntryRequest;

/// <summary>
/// MediatR request to add a new date entry to a calendar.
/// </summary>
public class AddDateEntryToDbRequest : IRequest<AddDateEntryToDbResponse>
{
    public AddDateEntryToDbRequest(
        Guid calendarId,
        string description,
        DateOnly startDate,
        bool isRecurring,
        int duration,
        int? frequency,
        int numberOfRepetitions)
    {
        CalendarId = calendarId;
        Description = description;
        StartDate = startDate;
        IsRecurring = isRecurring;
        Duration = duration;
        Frequency = frequency;
        NumberOfRepetitions = numberOfRepetitions;
    }

    public Guid CalendarId { get; }
    public string Description { get; }
    public DateOnly StartDate { get; }
    public bool IsRecurring { get; }
    public int Duration { get; }
    public int? Frequency { get; }
    public int NumberOfRepetitions { get; }
}

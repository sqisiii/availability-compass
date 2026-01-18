using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateDateEntryRequest;

/// <summary>
/// MediatR request to update an existing date entry in the database.
/// </summary>
public class UpdateDateEntryInDbRequest : IRequest<UpdateDateEntryInDbResponse>
{
    public UpdateDateEntryInDbRequest(
        Guid calendarId,
        Guid dateEntryId,
        string description,
        DateOnly startDate,
        bool isRecurring,
        int duration,
        int? frequency,
        int numberOfRepetitions)
    {
        CalendarId = calendarId;
        DateEntryId = dateEntryId;
        Description = description;
        StartDate = startDate;
        IsRecurring = isRecurring;
        Duration = duration;
        Frequency = frequency;
        NumberOfRepetitions = numberOfRepetitions;
    }

    public Guid CalendarId { get; }
    public Guid DateEntryId { get; }
    public string Description { get; }
    public DateOnly StartDate { get; }
    public bool IsRecurring { get; }
    public int Duration { get; }
    public int? Frequency { get; }
    public int NumberOfRepetitions { get; }
}

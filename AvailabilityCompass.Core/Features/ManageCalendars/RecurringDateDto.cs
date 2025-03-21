namespace AvailabilityCompass.Core.Features.ManageCalendars;

public record RecurringDateDto(
    Guid RecurringDateId,
    string RecurringDateDescription,
    DateOnly StartDate,
    int Duration,
    int RepetitionPeriod,
    int NumberOfRepetitions,
    DateTime ChangeDate)
{
    public RecurringDateDto() : this(Guid.Empty, string.Empty, DateOnly.MinValue, 1, 1, 1, DateTime.MinValue)
    {
    }
}
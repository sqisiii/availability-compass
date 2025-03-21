namespace AvailabilityCompass.Core.Features.ManageCalendars;

public record RecurringDateDto(Guid RecurringDateId, string RecurringDateDescription, DateOnly StartDate, int DaysCount)
{
    public RecurringDateDto() : this(Guid.Empty, string.Empty, DateOnly.MinValue, 0)
    {
    }
}
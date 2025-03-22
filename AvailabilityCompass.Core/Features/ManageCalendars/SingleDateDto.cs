namespace AvailabilityCompass.Core.Features.ManageCalendars;

public record SingleDateDto(Guid CalendarId, Guid SingleDateId, string SingleDateDescription, DateOnly Date, DateTime ChangeDate)
{
    public SingleDateDto() : this(Guid.Empty, Guid.Empty, string.Empty, DateOnly.MinValue, DateTime.MinValue)
    {
    }
}
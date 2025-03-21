namespace AvailabilityCompass.Core.Features.ManageCalendars;

public record SingleDateDto(Guid SingleDateId, string SingleDateDescription, DateOnly Date)
{
    public SingleDateDto() : this(Guid.Empty, string.Empty, DateOnly.MinValue)
    {
    }
}
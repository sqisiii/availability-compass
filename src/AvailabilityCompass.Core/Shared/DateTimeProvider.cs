namespace AvailabilityCompass.Core.Shared;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
}

public interface IDateTimeProvider
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}
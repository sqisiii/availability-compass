namespace AvailabilityCompass.Core.Shared;

/// <summary>
/// Default implementation of <see cref="IDateTimeProvider"/> using system time.
/// </summary>
public class DateTimeProvider : IDateTimeProvider
{
    /// <inheritdoc />
    public DateTime Now => DateTime.Now;

    /// <inheritdoc />
    public DateTime UtcNow => DateTime.UtcNow;
}

/// <summary>
/// Provides an abstraction for accessing the current date and time, enabling testability.
/// </summary>
public interface IDateTimeProvider
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
}
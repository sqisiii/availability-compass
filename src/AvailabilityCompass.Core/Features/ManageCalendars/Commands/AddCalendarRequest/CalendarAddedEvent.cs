namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddCalendarRequest;

/// <summary>
/// Event published when a new calendar is added to the database.
/// </summary>
public record CalendarAddedEvent(Guid CalendarId);
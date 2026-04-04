namespace AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateCalendarRequest;

/// <summary>
/// Event published when a calendar is updated in the database.
/// </summary>
public record CalendarUpdatedEvent(
    Guid CalendarId,
    string Name,
    bool IsOnly);
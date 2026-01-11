namespace AvailabilityCompass.Core.Features.ManageCalendars;

/// <summary>
/// Factory interface for creating calendar-related view models.
/// </summary>
public interface ICalendarViewModelFactory
{
    /// <summary>
    /// Creates a CalendarViewModel from a CalendarDto.
    /// </summary>
    /// <param name="calendarDto">The data transfer object containing calendar data.</param>
    /// <returns>A CalendarViewModel instance.</returns>
    CalendarViewModel CreateCalendar(CalendarDto calendarDto);

    /// <summary>
    /// Creates a DateEntryViewModel from a DateEntryDto.
    /// </summary>
    /// <param name="dateEntryDto">The data transfer object containing date entry data.</param>
    /// <returns>A DateEntryViewModel instance.</returns>
    DateEntryViewModel CreateDateEntry(DateEntryDto dateEntryDto);
}

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
    /// Creates a SingleDateViewModel from a SingleDateDto.
    /// </summary>
    /// <param name="singleDateDto">The data transfer object containing single date data.</param>
    /// <returns>A SingleDateViewModel instance.</returns>
    SingleDateViewModel CreateSingleDate(SingleDateDto singleDateDto);

    /// <summary>
    /// Creates a RecurringDateViewModel from a RecurringDateDto.
    /// </summary>
    /// <param name="recurringDateDto">The data transfer object containing recurring date data.</param>
    /// <returns>A RecurringDateViewModel instance.</returns>
    RecurringDateViewModel CreateRecurringDate(RecurringDateDto recurringDateDto);
}

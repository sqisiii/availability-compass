namespace AvailabilityCompass.Core.Features.ManageCalendars.DatesCalculator;

/// <summary>
/// Interface for calculating reserved dates.
/// </summary>
public interface IReservedDatesCalculator
{
    /// <summary>
    /// Gets the reserved categorized days for a selected calendar.
    /// </summary>
    /// <param name="selectedCalendar">The selected calendar view model.</param>
    /// <returns>A list of categorized dates.</returns>
    List<CategorizedDate> GetReservedCategorizedDays(CalendarViewModel? selectedCalendar);

    /// <summary>
    /// Gets the reserved dates for a list of calendars.
    /// </summary>
    /// <param name="calendars">The list of calendar DTOs to process.</param>
    /// <returns>A list of reserved dates.</returns>
    List<DateOnly> GetReservedDates(List<CalendarDto> calendars);
}
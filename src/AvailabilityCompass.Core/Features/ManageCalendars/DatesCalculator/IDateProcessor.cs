namespace AvailabilityCompass.Core.Features.ManageCalendars.DatesCalculator;

/// <summary>
/// Defines methods for processing calendar data to extract categorized dates.
/// </summary>
/// <remarks>
/// Implementations of this interface are responsible for analyzing calendar information
/// and creating a list of dates with their respective categories.
/// </remarks>
public interface IDateProcessor
{
    /// <summary>
    /// Processes calendar data to produce a list of categorized dates.
    /// </summary>
    /// <param name="calendar">The calendar view model to process.</param>
    /// <returns>A list of dates with their respective categories.</returns>
    List<CategorizedDate> Process(CalendarViewModel calendar);
}
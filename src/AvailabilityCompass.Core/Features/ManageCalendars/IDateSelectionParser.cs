namespace AvailabilityCompass.Core.Features.ManageCalendars;

/// <summary>
/// Parses calendar date selections and detects consecutive date periods.
/// </summary>
public interface IDateSelectionParser
{
    /// <summary>
    /// Analyzes selected dates and groups consecutive dates into periods.
    /// Non-consecutive dates are returned as separate selections.
    /// </summary>
    /// <param name="selectedDates">Collection of selected DateTime values (can be unsorted).</param>
    /// <returns>List of detected selections, where consecutive dates are grouped into periods.</returns>
    List<DetectedSelection> ParseSelectedDates(IEnumerable<DateTime> selectedDates);
}

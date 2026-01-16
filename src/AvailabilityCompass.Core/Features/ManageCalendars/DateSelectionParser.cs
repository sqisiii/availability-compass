namespace AvailabilityCompass.Core.Features.ManageCalendars;

/// <summary>
/// Parses calendar date selections and detects consecutive date periods.
/// </summary>
public class DateSelectionParser : IDateSelectionParser
{
    /// <inheritdoc />
    public List<DetectedSelection> ParseSelectedDates(IEnumerable<DateTime> selectedDates)
    {
        var sortedDates = selectedDates
            .Select(DateOnly.FromDateTime)
            .OrderBy(d => d)
            .ToList();

        var selections = new List<DetectedSelection>();

        if (sortedDates.Count == 0)
        {
            return selections;
        }

        var currentStart = sortedDates[0];
        var currentDuration = 1;

        for (var i = 1; i < sortedDates.Count; i++)
        {
            var expected = sortedDates[i - 1].AddDays(1);
            if (sortedDates[i] == expected)
            {
                currentDuration++;
            }
            else
            {
                selections.Add(new DetectedSelection(currentStart, currentDuration));
                currentStart = sortedDates[i];
                currentDuration = 1;
            }
        }

        selections.Add(new DetectedSelection(currentStart, currentDuration));

        return selections;
    }
}
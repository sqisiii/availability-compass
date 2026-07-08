namespace AvailabilityCompass.Core.Features.ManageCalendars.DatesCalculator;

/// <summary>
/// Processes date entries from a calendar to produce categorized dates for display.
/// </summary>
public class DateEntryProcessor : IDateProcessor
{
    /// <inheritdoc />
    public List<CategorizedDate> Process(CalendarViewModel calendar)
    {
        var result = new List<CategorizedDate>();

        foreach (var dateEntry in calendar.DateEntries)
        {
            if (dateEntry.IsRecurring)
            {
                ProcessRecurring(dateEntry, result);
            }
            else
            {
                ProcessSingle(dateEntry, result);
            }
        }

        return result;
    }

    private static void ProcessSingle(DateEntryViewModel dateEntry, List<CategorizedDate> result)
    {
        var currentDate = dateEntry.StartDate;
        for (var i = 0; i < dateEntry.Duration; i++)
        {
            result.Add(new CategorizedDate(
                currentDate.ToDateTime(TimeOnly.MinValue),
                CategorizedDateCategory.SingleDate,
                dateEntry.Description));
            currentDate = currentDate.AddDays(1);
        }
    }

    private static void ProcessRecurring(DateEntryViewModel dateEntry, List<CategorizedDate> result)
    {
        var currentDate = dateEntry.StartDate;
        // Clamp persisted values too — validation only covers newly entered data.
        var numberOfRepetitions = Math.Min(dateEntry.NumberOfRepetitions, DateEntryLimits.MaxRepetitions);
        var added = 0;

        for (var i = 0; i <= numberOfRepetitions && added < DateEntryLimits.MaxExpandedDatesPerEntry; i++)
        {
            var durationDate = currentDate;
            var duration = dateEntry.Duration;

            for (var j = 0; j < duration && added < DateEntryLimits.MaxExpandedDatesPerEntry; j++)
            {
                result.Add(new CategorizedDate(
                    durationDate.ToDateTime(TimeOnly.MinValue),
                    CategorizedDateCategory.RecurringDate,
                    dateEntry.Description));

                durationDate = durationDate.AddDays(1);
                added++;
            }

            // A non-positive frequency would re-add the same dates; an advance past
            // DateOnly.MaxValue would throw.
            if (dateEntry.Frequency is not ({ } frequency and > 0)
                || currentDate.DayNumber + frequency > DateOnly.MaxValue.DayNumber)
            {
                break;
            }

            currentDate = currentDate.AddDays(frequency);
        }
    }
}
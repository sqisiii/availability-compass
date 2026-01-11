namespace AvailabilityCompass.Core.Features.ManageCalendars.DatesCalculator;

public class DateEntryProcessor : IDateProcessor
{
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
        result.Add(new CategorizedDate(
            dateEntry.StartDate.ToDateTime(TimeOnly.MinValue),
            CategorizedDateCategory.SingleDate,
            dateEntry.Description));
    }

    private static void ProcessRecurring(DateEntryViewModel dateEntry, List<CategorizedDate> result)
    {
        var currentDate = dateEntry.StartDate;
        var numberOfRepetitions = dateEntry.NumberOfRepetitions;

        for (var i = 0; i <= numberOfRepetitions; i++)
        {
            var durationDate = currentDate;
            var duration = dateEntry.Duration;

            for (var j = 0; j < duration; j++)
            {
                result.Add(new CategorizedDate(
                    durationDate.ToDateTime(TimeOnly.MinValue),
                    CategorizedDateCategory.RecurringDate,
                    dateEntry.Description));

                durationDate = durationDate.AddDays(1);
            }

            if (dateEntry.Frequency is not null)
            {
                currentDate = currentDate.AddDays(dateEntry.Frequency.Value);
            }
        }
    }
}
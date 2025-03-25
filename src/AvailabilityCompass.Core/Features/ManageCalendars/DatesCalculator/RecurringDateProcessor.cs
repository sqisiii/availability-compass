namespace AvailabilityCompass.Core.Features.ManageCalendars.DatesCalculator;

public class RecurringDateProcessor : IDateProcessor
{
    private readonly int _defaultDuration = 1;
    private readonly int _defaultNumberOfRepetitions = 0;

    public List<CategorizedDate> Process(CalendarViewModel calendar)
    {
        var result = new List<CategorizedDate>();

        foreach (var recurringDate in calendar.RecurringDates)
        {
            if (recurringDate.StartDate is null)
            {
                continue;
            }

            var currentDate = recurringDate.StartDate;
            var numberOfRepetitions = recurringDate.NumberOfRepetitions ?? _defaultNumberOfRepetitions;

            for (var i = 0; i <= numberOfRepetitions; i++)
            {
                var durationDate = currentDate;
                var duration = recurringDate.Duration ?? _defaultDuration;

                for (var j = 0; j < duration; j++)
                {
                    result.Add(new CategorizedDate(
                        durationDate.Value.ToDateTime(TimeOnly.MinValue),
                        CategorizedDateCategory.RecurringDate,
                        recurringDate.Description ?? string.Empty));

                    durationDate = durationDate.Value.AddDays(1);
                }

                if (recurringDate.Frequency is not null)
                {
                    currentDate = currentDate.Value.AddDays(recurringDate.Frequency.Value);
                }
            }
        }

        return result;
    }
}
namespace AvailabilityCompass.Core.Features.ManageCalendars.DatesCalculator;

public class RecurringDateProcessor : IDateProcessor
{
    public List<CategorizedDate> Process(CalendarViewModel calendar)
    {
        var result = new List<CategorizedDate>();
        const int dateRelatedDefaultValue = 1;

        foreach (var recurringDate in calendar.RecurringDates)
        {
            if (recurringDate.StartDate is null)
            {
                continue;
            }

            var currentDate = recurringDate.StartDate;
            var numberOfRepetitions = recurringDate.NumberOfRepetitions ?? dateRelatedDefaultValue;

            for (var i = 0; i < numberOfRepetitions; i++)
            {
                var durationDate = currentDate;
                var duration = recurringDate.Duration ?? dateRelatedDefaultValue;

                for (var j = 0; j < duration; j++)
                {
                    result.Add(new CategorizedDate(
                        durationDate.Value.ToDateTime(TimeOnly.MinValue),
                        CategorizedDateCategory.RecurringDate,
                        recurringDate.Description ?? string.Empty));

                    durationDate = durationDate.Value.AddDays(1);
                }

                currentDate = currentDate.Value.AddDays(recurringDate.RepetitionPeriod ?? dateRelatedDefaultValue);
            }
        }

        return result;
    }
}
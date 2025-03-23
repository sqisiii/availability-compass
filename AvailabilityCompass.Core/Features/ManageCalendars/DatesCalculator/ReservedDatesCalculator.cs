namespace AvailabilityCompass.Core.Features.ManageCalendars.DatesCalculator;

public class ReservedDatesCalculator : IReservedDatesCalculator
{
    private readonly List<IDateProcessor> _dateProcessors =
    [
        new SingleDateProcessor(),
        new RecurringDateProcessor()
    ];

    public List<CategorizedDate> GetReservedCategorizedDays(CalendarViewModel? selectedCalendar)
    {
        if (selectedCalendar is null)
        {
            return [];
        }

        var allDates = new List<CategorizedDate>();
        foreach (var processor in _dateProcessors)
        {
            allDates.AddRange(processor.Process(selectedCalendar));
        }

        var uniqueDates = allDates
            .GroupBy(d => d.Date.Date)
            .Select(group => new CategorizedDate(
                group.First().Date,
                group.First().Category,
                string.Join(", ", group.Select(d => d.Tooltip).Distinct())))
            .ToList();

        return uniqueDates;
    }

    public List<DateOnly> GetReservedDates(List<CalendarDto> calendars)
    {
        if (calendars.Count == 0)
        {
            return [];
        }

        // Use HashSet to automatically handle duplicates
        var allDates = new HashSet<DateOnly>();

        foreach (var calendar in calendars)
        {
            foreach (var singleDate in calendar.SingleDates)
            {
                allDates.Add(singleDate.Date);
            }

            foreach (var recurringDate in calendar.RecurringDates)
            {
                var dates = CalculateRecurringDatesRange(
                    recurringDate.StartDate,
                    recurringDate.Duration,
                    recurringDate.RepetitionPeriod,
                    recurringDate.NumberOfRepetitions);

                foreach (var date in dates)
                {
                    allDates.Add(date);
                }
            }
        }

        return allDates.ToList();
    }

    private List<DateOnly> CalculateRecurringDatesRange(
        DateOnly startDate,
        int duration,
        int repetitionPeriod,
        int numberOfRepetitions)
    {
        List<DateOnly> result = [];
        var currentDate = startDate;

        for (var i = 0; i < numberOfRepetitions; i++)
        {
            var durationDate = currentDate;

            for (var j = 0; j < duration; j++)
            {
                result.Add(durationDate);
                durationDate = durationDate.AddDays(1);
            }

            currentDate = currentDate.AddDays(repetitionPeriod);
        }

        return result;
    }
}
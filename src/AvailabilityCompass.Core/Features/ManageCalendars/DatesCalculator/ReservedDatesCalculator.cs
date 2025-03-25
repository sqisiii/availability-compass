namespace AvailabilityCompass.Core.Features.ManageCalendars.DatesCalculator;

public class ReservedDatesCalculator : IReservedDatesCalculator
{
    private const int _yearsToCheck = 2;

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

        // If the calendar is an "only" calendar, invert the selection
        if (selectedCalendar.IsOnly)
        {
            uniqueDates = GetInvertedDates(uniqueDates);
        }

        return uniqueDates;
    }

    public List<DateOnly> GetReservedDates(List<CalendarDto> calendars)
    {
        if (calendars.Count == 0)
        {
            return [];
        }

        var normalCalendarsReservedDates = new HashSet<DateOnly>();
        var normalCalendars = calendars.Where(c => !c.IsOnly).ToList();
        foreach (var calendar in normalCalendars)
        {
            foreach (var singleDate in calendar.SingleDates)
            {
                normalCalendarsReservedDates.Add(singleDate.Date);
            }

            foreach (var recurringDate in calendar.RecurringDates)
            {
                var dates = CalculateRecurringDatesRange(
                    recurringDate.StartDate,
                    recurringDate.Duration,
                    recurringDate.Frequency,
                    recurringDate.NumberOfRepetitions);

                foreach (var date in dates)
                {
                    normalCalendarsReservedDates.Add(date);
                }
            }
        }

        // Process "Only" calendars by inverting dates
        var onlyCalendarsExceptionDates = new HashSet<DateOnly>();
        var onlyCalendars = calendars.Where(c => c.IsOnly).ToList();
        foreach (var calendar in onlyCalendars)
        {
            foreach (var singleDate in calendar.SingleDates)
            {
                onlyCalendarsExceptionDates.Add(singleDate.Date);
            }

            foreach (var recurringDate in calendar.RecurringDates)
            {
                var dates = CalculateRecurringDatesRange(
                    recurringDate.StartDate,
                    recurringDate.Duration,
                    recurringDate.Frequency,
                    recurringDate.NumberOfRepetitions);

                foreach (var date in dates)
                {
                    onlyCalendarsExceptionDates.Add(date);
                }
            }
        }

        // If there are no "Only" calendars, just return the dates from normal calendars
        if (onlyCalendars.Count == 0)
        {
            return normalCalendarsReservedDates.ToList();
        }

        // If there are any "Only" calendars, compute all dates for the next year
        // and exclude the exception dates
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate.AddYears(_yearsToCheck);
        var allDatesInNextYear = new HashSet<DateOnly>();

        for (var date = startDate; date < endDate; date = date.AddDays(1))
        {
            if (!onlyCalendarsExceptionDates.Contains(date))
            {
                allDatesInNextYear.Add(date);
            }
        }

        // Combine the dates from normal calendars with inverted dates from "Only" calendars
        return normalCalendarsReservedDates.Union(allDatesInNextYear).ToList();
    }

    private List<CategorizedDate> GetInvertedDates(List<CategorizedDate> dates)
    {
        var exceptedDates = dates.Select(d => DateOnly.FromDateTime(d.Date.Date)).ToHashSet();
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate.AddYears(_yearsToCheck);
        List<CategorizedDate> invertedDates = [];

        for (var date = startDate; date < endDate; date = date.AddDays(1))
        {
            if (!exceptedDates.Contains(date))
            {
                invertedDates.Add(new CategorizedDate(
                    date.ToDateTime(TimeOnly.MinValue),
                    CategorizedDateCategory.Inverted,
                    "Inverted date"));
            }
        }

        return invertedDates;
    }

    private List<DateOnly> CalculateRecurringDatesRange(
        DateOnly startDate,
        int duration,
        int? frequency,
        int numberOfRepetitions)
    {
        List<DateOnly> result = [];
        var currentDate = startDate;

        for (var i = 0; i <= numberOfRepetitions; i++)
        {
            var durationDate = currentDate;

            for (var j = 0; j < duration; j++)
            {
                result.Add(durationDate);
                durationDate = durationDate.AddDays(1);
            }

            if (frequency is not null)
            {
                currentDate = currentDate.AddDays(frequency.Value);
            }
        }

        return result;
    }
}
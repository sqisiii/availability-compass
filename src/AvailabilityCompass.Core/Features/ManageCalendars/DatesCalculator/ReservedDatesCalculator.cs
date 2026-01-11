namespace AvailabilityCompass.Core.Features.ManageCalendars.DatesCalculator;

public class ReservedDatesCalculator : IReservedDatesCalculator
{
    private const int YearsToCheck = 2;

    private readonly IDateProcessor _dateProcessor;

    public ReservedDatesCalculator(IDateProcessor dateProcessor)
    {
        _dateProcessor = dateProcessor;
    }

    public List<CategorizedDate> GetReservedCategorizedDays(CalendarViewModel? selectedCalendar)
    {
        if (selectedCalendar is null)
        {
            return [];
        }

        var allDates = _dateProcessor.Process(selectedCalendar);

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

        var normalCalendarsReservedDates = new HashSet<DateOnly>();
        var normalCalendars = calendars.Where(c => !c.IsOnly).ToList();
        foreach (var calendar in normalCalendars)
        {
            foreach (var dateEntry in calendar.DateEntries)
            {
                if (dateEntry.IsRecurring)
                {
                    var dates = CalculateRecurringDatesRange(
                        dateEntry.StartDate,
                        dateEntry.Duration,
                        dateEntry.Frequency,
                        dateEntry.NumberOfRepetitions);

                    foreach (var date in dates)
                    {
                        normalCalendarsReservedDates.Add(date);
                    }
                }
                else
                {
                    normalCalendarsReservedDates.Add(dateEntry.StartDate);
                }
            }
        }

        // Process "Only" calendars by inverting dates
        var onlyCalendarsExceptionDates = new HashSet<DateOnly>();
        var onlyCalendars = calendars.Where(c => c.IsOnly).ToList();
        foreach (var calendar in onlyCalendars)
        {
            foreach (var dateEntry in calendar.DateEntries)
            {
                if (dateEntry.IsRecurring)
                {
                    var dates = CalculateRecurringDatesRange(
                        dateEntry.StartDate,
                        dateEntry.Duration,
                        dateEntry.Frequency,
                        dateEntry.NumberOfRepetitions);

                    foreach (var date in dates)
                    {
                        onlyCalendarsExceptionDates.Add(date);
                    }
                }
                else
                {
                    onlyCalendarsExceptionDates.Add(dateEntry.StartDate);
                }
            }
        }

        // If there are no "Only" calendars, just return the dates from normal calendars
        if (onlyCalendars.Count == 0)
        {
            return normalCalendarsReservedDates.ToList();
        }

        // If there are any "Only" calendars, compute all dates for the next years
        // and exclude the exception dates
        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate.AddYears(YearsToCheck);
        var allDatesInRange = new HashSet<DateOnly>();

        for (var date = startDate; date < endDate; date = date.AddDays(1))
        {
            if (!onlyCalendarsExceptionDates.Contains(date))
            {
                allDatesInRange.Add(date);
            }
        }

        // Combine the dates from normal calendars with inverted dates from "Only" calendars
        return normalCalendarsReservedDates.Union(allDatesInRange).ToList();
    }

    private static List<DateOnly> CalculateRecurringDatesRange(
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
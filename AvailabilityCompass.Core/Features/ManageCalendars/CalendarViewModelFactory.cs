namespace AvailabilityCompass.Core.Features.ManageCalendars;

public class CalendarViewModelFactory : ICalendarViewModelFactory
{
    public CalendarViewModel CreateCalendar(CalendarDto calendarDto)
    {
        var viewModel = new CalendarViewModel
        {
            Name = calendarDto.Name,
            IsOnly = calendarDto.IsOnly,
            CalendarId = calendarDto.CalendarId,
            IsSelected = false,
            SingleDates = calendarDto.SingleDates
                .Select(CreateSingleDate)
                .ToList(),
            RecurringDates = calendarDto.RecurringDates
                .Select(CreateRecurringDate)
                .ToList()
        };

        return viewModel;
    }

    public SingleDateViewModel CreateSingleDate(SingleDateDto singleDateDto)
    {
        return new SingleDateViewModel
        {
            Id = singleDateDto.SingleDateId,
            Description = singleDateDto.SingleDateDescription,
            Date = singleDateDto.Date
        };
    }

    public RecurringDateViewModel CreateRecurringDate(RecurringDateDto recurringDateDto)
    {
        return new RecurringDateViewModel
        {
            Id = recurringDateDto.RecurringDateId,
            Description = recurringDateDto.RecurringDateDescription,
            StartDate = recurringDateDto.StartDate,
            Duration = recurringDateDto.Duration,
            RepetitionPeriod = recurringDateDto.RepetitionPeriod,
            NumberOfRepetitions = recurringDateDto.NumberOfRepetitions
        };
    }
}
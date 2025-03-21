namespace AvailabilityCompass.Core.Features.ManageCalendars;

public class CalendarViewModelFactory : ICalendarViewModelFactory
{
    public CalendarViewModel Create(CalendarDto calendarDto)
    {
        var viewModel = new CalendarViewModel
        {
            Name = calendarDto.Name,
            Id = calendarDto.Id,
            IsSelected = false,
            SingleDates = calendarDto.SingleDates
                .Select(sd => new SingleDateViewModel
                {
                    Id = sd.SingleDateId,
                    Description = sd.SingleDateDescription,
                    Date = sd.Date
                })
                .ToList(),
            RecurringDates = calendarDto.RecurringDates
                .Select(rd => new RecurringDateViewModel
                {
                    Id = rd.RecurringDateId,
                    Description = rd.RecurringDateDescription,
                    StartDate = rd.StartDate,
                    DaysCount = rd.DaysCount
                })
                .ToList()
        };

        return viewModel;
    }
}
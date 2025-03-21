namespace AvailabilityCompass.Core.Features.ManageCalendars;

public interface ICalendarViewModelFactory
{
    CalendarViewModel CreateCalendar(CalendarDto calendarDto);
    SingleDateViewModel CreateSingleDate(SingleDateDto singleDateDto);

    RecurringDateViewModel CreateRecurringDate(RecurringDateDto recurringDateDto);
}
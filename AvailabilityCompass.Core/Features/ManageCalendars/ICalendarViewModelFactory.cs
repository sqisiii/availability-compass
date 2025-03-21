namespace AvailabilityCompass.Core.Features.ManageCalendars;

public interface ICalendarViewModelFactory
{
    CalendarViewModel Create(CalendarDto calendarDto);
}
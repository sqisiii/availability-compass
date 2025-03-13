using AvailabilityCompass.Core.Features.Search.Queries.GetCalendars;

namespace AvailabilityCompass.Core.Features.Search;

public interface ICalendarViewModelFactory
{
    IEnumerable<CalendarViewModel> Create(IEnumerable<GetCalendarsForFilteringDto> calendars);
}
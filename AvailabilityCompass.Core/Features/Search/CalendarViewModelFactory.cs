using AvailabilityCompass.Core.Features.Search.Queries.GetCalendars;

namespace AvailabilityCompass.Core.Features.Search;

public class CalendarViewModelFactory : ICalendarViewModelFactory
{
    public IEnumerable<CalendarViewModel> Create(IEnumerable<GetCalendarsForFilteringDto> calendars)
    {
        return calendars.Select(c => new CalendarViewModel(c.Id) { Name = c.Name, Type = c.Type });
    }
}
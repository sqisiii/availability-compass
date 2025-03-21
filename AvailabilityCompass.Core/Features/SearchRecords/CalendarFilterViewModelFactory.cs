using AvailabilityCompass.Core.Features.SearchRecords.Queries.GetCalendars;

namespace AvailabilityCompass.Core.Features.SearchRecords;

public class CalendarFilterViewModelFactory : ICalendarFilterViewModelFactory
{
    public IEnumerable<CalendarFilterViewModel> Create(IEnumerable<GetCalendarsForFilteringResponse.CalendarDto> calendars)
    {
        return calendars.Select(c => new CalendarFilterViewModel(c.Id) { Name = c.Name, IsOnly = c.IsOnly });
    }
}
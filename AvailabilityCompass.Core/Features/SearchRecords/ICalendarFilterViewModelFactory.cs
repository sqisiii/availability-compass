using AvailabilityCompass.Core.Features.SearchRecords.Queries.GetCalendars;

namespace AvailabilityCompass.Core.Features.SearchRecords;

public interface ICalendarFilterViewModelFactory
{
    IEnumerable<CalendarFilterViewModel> Create(IEnumerable<GetCalendarsForFilteringDto> calendars);
}
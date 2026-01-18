using AvailabilityCompass.Core.Features.SearchRecords.Queries.GetCalendars;

namespace AvailabilityCompass.Core.Features.SearchRecords;

/// <summary>
/// Factory for creating CalendarFilterViewModel instances from calendar DTOs.
/// </summary>
public class CalendarFilterViewModelFactory : ICalendarFilterViewModelFactory
{
    /// <inheritdoc />
    public IEnumerable<CalendarFilterViewModel> Create(IEnumerable<GetCalendarsForFilteringResponse.CalendarDto> calendars)
    {
        return calendars.Select(c => new CalendarFilterViewModel(c.Id) { Name = c.Name, IsOnly = c.IsOnly });
    }
}
using AvailabilityCompass.Core.Features.SearchRecords.Queries.GetCalendars;

namespace AvailabilityCompass.Core.Features.SearchRecords;

/// <summary>
/// Factory interface for creating instances of <see cref="CalendarFilterViewModel"/>.
/// </summary>
public interface ICalendarFilterViewModelFactory
{
    /// <summary>
    /// Creates a collection of <see cref="CalendarFilterViewModel"/> from a collection of <see cref="GetCalendarsForFilteringResponse.CalendarDto"/>.
    /// </summary>
    /// <param name="calendars">The collection of <see cref="GetCalendarsForFilteringResponse.CalendarDto"/> to convert.</param>
    /// <returns>A collection of <see cref="CalendarFilterViewModel"/>.</returns>
    IEnumerable<CalendarFilterViewModel> Create(IEnumerable<GetCalendarsForFilteringResponse.CalendarDto> calendars);
}
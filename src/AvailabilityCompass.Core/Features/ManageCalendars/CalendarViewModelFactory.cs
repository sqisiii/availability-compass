namespace AvailabilityCompass.Core.Features.ManageCalendars;

/// <summary>
/// Factory for creating calendar and date entry view models from DTOs.
/// </summary>
public class CalendarViewModelFactory : ICalendarViewModelFactory
{
    /// <inheritdoc />
    public CalendarViewModel CreateCalendar(CalendarDto calendarDto)
    {
        var viewModel = new CalendarViewModel
        {
            Name = calendarDto.Name,
            IsOnly = calendarDto.IsOnly,
            CalendarId = calendarDto.CalendarId,
            IsSelected = false,
            DateEntries = calendarDto.DateEntries
                .Select(CreateDateEntry)
                .ToList()
        };

        return viewModel;
    }

    /// <inheritdoc />
    public DateEntryViewModel CreateDateEntry(DateEntryDto dateEntryDto)
    {
        return new DateEntryViewModel
        {
            DateEntryId = dateEntryDto.DateEntryId,
            CalendarId = dateEntryDto.CalendarId,
            Description = dateEntryDto.Description,
            StartDate = dateEntryDto.StartDate,
            IsRecurring = dateEntryDto.IsRecurring,
            Duration = dateEntryDto.Duration,
            Frequency = dateEntryDto.Frequency,
            NumberOfRepetitions = dateEntryDto.NumberOfRepetitions
        };
    }
}

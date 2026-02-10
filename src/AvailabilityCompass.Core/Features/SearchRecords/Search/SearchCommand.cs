using AvailabilityCompass.Core.Features.ManageCalendars;
using AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetCalendarsWithEntries;
using AvailabilityCompass.Core.Features.SearchRecords.Events;
using AvailabilityCompass.Core.Features.SearchRecords.FilterFormElements;
using AvailabilityCompass.Core.Features.SearchRecords.Queries.GetAvailableDates;
using AvailabilityCompass.Core.Features.SearchRecords.Queries.SearchSources;
using AvailabilityCompass.Core.Shared.EventBus;
using MediatR;

namespace AvailabilityCompass.Core.Features.SearchRecords.Search;

/// <summary>
/// Command that executes search operations against selected sources with calendar-based date filtering.
/// </summary>
public class SearchCommand : ISearchCommand
{
    private const int CalendarConflictYearsToCheck = 2;
    private readonly IEventBus _eventBus;
    private readonly IMediator _mediator;
    private readonly Lazy<SearchViewModel> _viewModel;

    public SearchCommand(
        Func<SearchViewModel> viewModelFactory,
        IMediator mediator,
        IEventBus eventBus)
    {
        _mediator = mediator;
        _eventBus = eventBus;
        _viewModel = new Lazy<SearchViewModel>(viewModelFactory);
    }

    /// <inheritdoc />
    public async Task ExecuteAsync()
    {
        if (!_viewModel.Value.Sources.Any(s => s.IsSelected))
        {
            return;
        }

        var selectedCalendarsForFiltering = GetSelectedCalendarsForFiltering();
        var availableDatesResponse = selectedCalendarsForFiltering.Count == 0
            ? new GetAvailableDatesResponse(true, [])
            : await GetAvailableDatesAsync(selectedCalendarsForFiltering);

        var selectedCalendarsForMarking = GetSelectedCalendarsForMarking();
        var selectedCalendarsWithEntries = await GetCalendarsWithEntriesAsync(selectedCalendarsForMarking);

        var query = CreateSearchSourcesQuery(availableDatesResponse.ReservedDates);
        AddSourceFilters(query);

        var searchResponse = await SearchAsync(query);

        var calendarOverlapSummaries = BuildCalendarOverlapSummaries(selectedCalendarsWithEntries);
        ProcessSearchResponse(searchResponse, calendarOverlapSummaries);
    }

    private async Task<SearchSourcesResponse> SearchAsync(SearchSourcesQuery query)
    {
        var searchResponse = await _mediator.Send(query);
        return searchResponse;
    }

    private List<Guid> GetSelectedCalendarsForFiltering()
    {
        return _viewModel.Value.Calendars
            .Where(c => c.IsSelected && !c.IsMarkOnly)
            .Select(c => c.Id)
            .ToList();
    }

    private List<Guid> GetSelectedCalendarsForMarking()
    {
        return _viewModel.Value.Calendars
            .Where(c => c.IsSelected && c.IsMarkOnly)
            .Select(c => c.Id)
            .ToList();
    }

    private async Task<List<CalendarDto>> GetCalendarsWithEntriesAsync(List<Guid> calendarIds)
    {
        if (calendarIds.Count == 0)
        {
            return [];
        }

        var calendarsWithEntriesResponse = await _mediator.Send(new GetCalendarsWithEntriesQuery(calendarIds));
        return calendarsWithEntriesResponse.IsSuccess ? calendarsWithEntriesResponse.Calendars : [];
    }

    private async Task<GetAvailableDatesResponse> GetAvailableDatesAsync(List<Guid> selectedCalendars)
    {
        return await _mediator.Send(new GetAvailableDatesQuery(selectedCalendars));
    }

    private SearchSourcesQuery CreateSearchSourcesQuery(List<DateOnly> reservedDates)
    {
        return new SearchSourcesQuery
        {
            ReservedDates = reservedDates
        };
    }

    private void AddSourceFilters(SearchSourcesQuery query)
    {
        foreach (var source in _viewModel.Value.Sources.Where(s => s.IsSelected))
        {
            var sourceFilters = new SearchSourcesQuery.Source(source.SourceId);
            AddFormGroupFilters(sourceFilters, source.SourceId);
            AddDateAndSearchPhraseFilters(sourceFilters);

            query.Sources.Add(sourceFilters);
        }
    }

    private void AddFormGroupFilters(SearchSourcesQuery.Source sourceFilters, string sourceId)
    {
        foreach (var formGroup in _viewModel.Value.FormGroups.Where(f => f.SourceId == sourceId))
        {
            foreach (var formElement in formGroup.Elements)
            {
                ApplyFilterStrategy(sourceFilters, formElement);
            }
        }
    }

    private void ApplyFilterStrategy(SearchSourcesQuery.Source sourceFilters, FormElement formElement)
    {
        IFilterStrategy strategy;
        switch (formElement.Type)
        {
            case FormElementType.MultiSelect:
                strategy = new MultiSelectFilterStrategy();
                break;
            case FormElementType.TextBox:
                strategy = new TextBoxFilterStrategy();
                break;
            default:
                throw new NotSupportedException($"Form element type {formElement.Type} is not supported");
        }

        strategy.ApplyFilter(sourceFilters, formElement);
    }

    private void AddDateAndSearchPhraseFilters(SearchSourcesQuery.Source sourceFilters)
    {
        if (!string.IsNullOrEmpty(_viewModel.Value.StartDate))
        {
            sourceFilters.SelectedFiltersValues.Add("startDate", [_viewModel.Value.StartDate]);
        }

        if (!string.IsNullOrEmpty(_viewModel.Value.EndDate))
        {
            sourceFilters.SelectedFiltersValues.Add("endDate", [_viewModel.Value.EndDate]);
        }

        if (!string.IsNullOrEmpty(_viewModel.Value.SearchPhrase))
        {
            sourceFilters.SelectedFiltersValues.Add("search", [_viewModel.Value.SearchPhrase]);
        }
    }

    private void ProcessSearchResponse(
        SearchSourcesResponse searchResponse,
        IReadOnlyDictionary<Guid, CalendarOverlapData> calendarOverlapSummaries)
    {
        _viewModel.Value.Columns.Clear();
        _viewModel.Value.Results.Clear();

        if (!searchResponse.IsSuccess || searchResponse.SourceDataItems.Count == 0)
        {
            return;
        }

        AddDefaultColumns();
        AddSearchResults(searchResponse.SourceDataItems, calendarOverlapSummaries);
        _viewModel.Value.OnUpdateColumns();

        _eventBus.Publish(new SearchResultsFoundEvent());
    }

    private void AddDefaultColumns()
    {
        _viewModel.Value.Columns.Add(new ResultColumnDefinition("Source", "SourceName"));
        _viewModel.Value.Columns.Add(new ResultColumnDefinition("Title", "Title"));
        _viewModel.Value.Columns.Add(new ResultColumnDefinition("Start Date", "StartDate"));
        _viewModel.Value.Columns.Add(new ResultColumnDefinition("End Date", "EndDate"));
    }

    private void AddSearchResults(
        IReadOnlyCollection<SearchSourcesResponse.SourceDataItem> sourceDataItems,
        IReadOnlyDictionary<Guid, CalendarOverlapData> calendarOverlapSummaries)
    {
        var sourceLookup = _viewModel.Value.Sources.ToDictionary(
            s => s.SourceId,
            s => new { s.Name, s.Language, s.IconPath });

        foreach (var sourceDataItem in sourceDataItems)
        {
            if (sourceDataItem.Title is null)
            {
                continue;
            }

            var sourceInfo = sourceLookup.GetValueOrDefault(sourceDataItem.SourceId);

            var singleSourceResults = new Dictionary<string, object>
            {
                { "SourceName", sourceInfo?.Name ?? sourceDataItem.SourceId },
                { "SourceLanguage", sourceInfo?.Language ?? string.Empty },
                { "SourceIconPath", sourceInfo?.IconPath ?? string.Empty },
                { "Title", sourceDataItem.Title },
                { "Url", sourceDataItem.Url ?? string.Empty },
                { "StartDate", sourceDataItem.StartDate.ToString("yyyy-MM-dd") },
                { "EndDate", sourceDataItem.EndDate.ToString("yyyy-MM-dd") }
            };

            AddAdditionalData(singleSourceResults, sourceDataItem.AdditionalData);
            AddCalendarOverlaps(singleSourceResults, sourceDataItem, calendarOverlapSummaries);
            _viewModel.Value.Results.Add(singleSourceResults);
        }

        _viewModel.Value.Columns.Add(new ResultColumnDefinition("URL", "Url"));
    }

    private void AddAdditionalData(Dictionary<string, object> singleSourceResults, Dictionary<string, object?> additionalData)
    {
        foreach (var (key, value) in additionalData)
        {
            if (_viewModel.Value.Columns.All(columnDefinition => columnDefinition.PropertyName != key))
            {
                _viewModel.Value.Columns.Add(new ResultColumnDefinition(key, key));
            }

            singleSourceResults.Add(key, value ?? string.Empty);
        }
    }

    private void AddCalendarOverlaps(
        Dictionary<string, object> singleSourceResults,
        SearchSourcesResponse.SourceDataItem sourceDataItem,
        IReadOnlyDictionary<Guid, CalendarOverlapData> calendarOverlapSummaries)
    {
        if (calendarOverlapSummaries.Count == 0)
        {
            return;
        }

        var overlaps = new List<CalendarOverlapSummary>();
        foreach (var overlapData in calendarOverlapSummaries.Values)
        {
            var conflictDates = overlapData.ConflictDates
                .Where(date => date >= sourceDataItem.StartDate && date <= sourceDataItem.EndDate)
                .OrderBy(date => date)
                .ToList();

            if (conflictDates.Count == 0)
            {
                continue;
            }

            var summary = FormatConflictSummary(conflictDates);
            overlaps.Add(new CalendarOverlapSummary(overlapData.CalendarName, summary));
        }

        if (overlaps.Count <= 0)
        {
            return;
        }

        singleSourceResults["CalendarOverlaps"] = overlaps;
        singleSourceResults["HasCalendarOverlaps"] = true;
    }

    private IReadOnlyDictionary<Guid, CalendarOverlapData> BuildCalendarOverlapSummaries(List<CalendarDto> calendars)
    {
        var selectedCalendars = _viewModel.Value.Calendars
            .Where(c => c.IsSelected && c.IsMarkOnly)
            .ToDictionary(c => c.Id, c => c.Name);

        if (selectedCalendars.Count == 0)
        {
            return new Dictionary<Guid, CalendarOverlapData>();
        }

        var result = new Dictionary<Guid, CalendarOverlapData>();
        foreach (var calendar in calendars.Where(c => selectedCalendars.ContainsKey(c.CalendarId)))
        {
            var conflictDates = GetConflictDates(calendar);
            if (conflictDates.Count == 0)
            {
                continue;
            }

            result[calendar.CalendarId] = new CalendarOverlapData(selectedCalendars[calendar.CalendarId], conflictDates);
        }

        return result;
    }

    private HashSet<DateOnly> GetConflictDates(CalendarDto calendar)
    {
        var conflictDates = new HashSet<DateOnly>();

        if (!calendar.IsOnly)
        {
            foreach (var dateEntry in calendar.DateEntries)
            {
                foreach (var date in ExpandDateEntry(dateEntry))
                {
                    conflictDates.Add(date);
                }
            }

            return conflictDates;
        }

        var exceptionDates = new HashSet<DateOnly>();
        foreach (var dateEntry in calendar.DateEntries)
        {
            foreach (var date in ExpandDateEntry(dateEntry))
            {
                exceptionDates.Add(date);
            }
        }

        var startDate = DateOnly.FromDateTime(DateTime.Today);
        var endDate = startDate.AddYears(CalendarConflictYearsToCheck);
        for (var date = startDate; date < endDate; date = date.AddDays(1))
        {
            if (!exceptionDates.Contains(date))
            {
                conflictDates.Add(date);
            }
        }

        return conflictDates;
    }

    private static List<DateOnly> ExpandDateEntry(DateEntryDto dateEntry)
    {
        var result = new List<DateOnly>();
        if (!dateEntry.IsRecurring)
        {
            var currentDate = dateEntry.StartDate;
            for (var i = 0; i < dateEntry.Duration; i++)
            {
                result.Add(currentDate);
                currentDate = currentDate.AddDays(1);
            }

            return result;
        }

        var currentRecurringDate = dateEntry.StartDate;
        for (var i = 0; i <= dateEntry.NumberOfRepetitions; i++)
        {
            var durationDate = currentRecurringDate;
            for (var j = 0; j < dateEntry.Duration; j++)
            {
                result.Add(durationDate);
                durationDate = durationDate.AddDays(1);
            }

            if (dateEntry.Frequency is not null)
            {
                currentRecurringDate = currentRecurringDate.AddDays(dateEntry.Frequency.Value);
            }
        }

        return result;
    }

    private static string FormatConflictSummary(List<DateOnly> conflictDates)
    {
        var grouped = conflictDates
            .GroupBy(d => new { d.Year, d.Month })
            .OrderBy(g => g.Key.Year)
            .ThenBy(g => g.Key.Month)
            .ToList();

        var lines = new List<string>();
        foreach (var group in grouped)
        {
            var monthName = new DateOnly(group.Key.Year, group.Key.Month, 1).ToString("MMM");
            var days = group.Select(d => d.Day).Distinct().OrderBy(d => d).ToList();
            lines.Add($"{monthName} {string.Join(" ", days)}");
        }

        return string.Join("\n", lines);
    }

    private sealed record CalendarOverlapData(string CalendarName, HashSet<DateOnly> ConflictDates);
}
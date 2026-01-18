using AvailabilityCompass.Core.Features.SearchRecords.FilterFormElements;
using AvailabilityCompass.Core.Features.SearchRecords.Queries.GetAvailableDates;
using AvailabilityCompass.Core.Features.SearchRecords.Queries.SearchSources;
using MediatR;

namespace AvailabilityCompass.Core.Features.SearchRecords.Search;

/// <summary>
/// Command that executes search operations against selected sources with calendar-based date filtering.
/// </summary>
public class SearchCommand : ISearchCommand
{
    private readonly IMediator _mediator;
    private readonly Lazy<SearchViewModel> _viewModel;

    public SearchCommand(Func<SearchViewModel> viewModelFactory, IMediator mediator)
    {
        _mediator = mediator;
        _viewModel = new Lazy<SearchViewModel>(viewModelFactory);
    }

    /// <inheritdoc />
    public async Task ExecuteAsync()
    {
        if (!_viewModel.Value.Sources.Any(s => s.IsSelected))
        {
            return;
        }

        var selectedCalendars = GetSelectedCalendars();
        var availableDatesResponse = await GetAvailableDatesAsync(selectedCalendars);

        var query = CreateSearchSourcesQuery(availableDatesResponse.ReservedDates);
        AddSourceFilters(query);

        var searchResponse = await SearchAsync(query);

        ProcessSearchResponse(searchResponse);
    }

    private async Task<SearchSourcesResponse> SearchAsync(SearchSourcesQuery query)
    {
        var searchResponse = await _mediator.Send(query);
        return searchResponse;
    }

    private List<Guid> GetSelectedCalendars()
    {
        return _viewModel.Value.Calendars.Where(c => c.IsSelected).Select(c => c.Id).ToList();
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

    private void ProcessSearchResponse(SearchSourcesResponse searchResponse)
    {
        _viewModel.Value.Columns.Clear();
        _viewModel.Value.Results.Clear();

        if (!searchResponse.IsSuccess || searchResponse.SourceDataItems.Count == 0)
        {
            return;
        }

        AddDefaultColumns();
        AddSearchResults(searchResponse.SourceDataItems);
        _viewModel.Value.OnUpdateColumns();
    }

    private void AddDefaultColumns()
    {
        _viewModel.Value.Columns.Add(new ResultColumnDefinition("Source", "SourceName"));
        _viewModel.Value.Columns.Add(new ResultColumnDefinition("Title", "Title"));
        _viewModel.Value.Columns.Add(new ResultColumnDefinition("Start Date", "StartDate"));
        _viewModel.Value.Columns.Add(new ResultColumnDefinition("End Date", "EndDate"));
    }

    private void AddSearchResults(IReadOnlyCollection<SearchSourcesResponse.SourceDataItem> sourceDataItems)
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
}
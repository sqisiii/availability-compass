using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AvailabilityCompass.Core.Features.Search.FilterFormElements;
using AvailabilityCompass.Core.Features.Search.Queries.GetCalendars;
using AvailabilityCompass.Core.Features.Search.Queries.GetSources;
using AvailabilityCompass.Core.Features.Search.Queries.SearchSources;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.ValidationAttributes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;

namespace AvailabilityCompass.Core.Features.Search;

public partial class SearchViewModel : ObservableValidator, IPageViewModel, IDisposable
{
    private readonly ICalendarViewModelFactory _calendarViewModelFactory;

    private readonly BehaviorSubject<List<ResultColumnDefinition>> _columnSubject = new([]);
    private readonly IFormElementFactory _formElementFactory;

    private readonly List<FormGroup> _formGroups = [];
    private readonly IMediator _mediator;
    private readonly ISourceViewModelFactory _sourceViewModelFactory;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [DateRangeValidation(nameof(StartDate))]
    [NotifyPropertyChangedFor(nameof(StartDate))]
    [DateValidation]
    private string? _endDate;

    private bool _isActive;

    [ObservableProperty]
    private string? _searchPhrase;

    [ObservableProperty]
    private object? _selectedResult;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
    private bool _sourceSelected;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [DateValidation]
    [DateRangeValidation(nameof(EndDate), true)]
    [NotifyPropertyChangedFor(nameof(EndDate))]
    private string? _startDate;


    public SearchViewModel(
        IMediator mediator,
        ISourceViewModelFactory sourceViewModelFactory,
        ICalendarViewModelFactory calendarViewModelFactory,
        IFormElementFactory formElementFactory)
    {
        _mediator = mediator;
        _sourceViewModelFactory = sourceViewModelFactory;
        _calendarViewModelFactory = calendarViewModelFactory;
        _formElementFactory = formElementFactory;
        Calendars.CollectionChanged += CalendarsOnCollectionChanged;
        Sources.CollectionChanged += SourcesOnCollectionChanged;
    }

    public IObservable<List<ResultColumnDefinition>> ColumnObservable => _columnSubject.AsObservable();

    public FullyObservableCollection<SourceViewModel> Sources { get; } = [];

    public FullyObservableCollection<CalendarViewModel> Calendars { get; } = [];

    public ObservableCollection<FormGroup> FormGroups { get; set; } = [];

    public ObservableCollection<ResultColumnDefinition> Columns { get; } = [];
    public ObservableCollection<Dictionary<string, object>> Results { get; } = [];

    public void Dispose()
    {
        Calendars.CollectionChanged -= CalendarsOnCollectionChanged;
        Sources.CollectionChanged -= SourcesOnCollectionChanged;
    }

    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive == value)
            {
                return;
            }

            _isActive = value;

            if (!_isActive)
            {
                return;
            }

            _ = LoadCalendarsAsync();
            _ = LoadSourcesAsync();
        }
    }

    public string Icon => "SearchWeb";
    public string Name => "Search";

    partial void OnEndDateChanged(string? value)
    {
        ValidateProperty(StartDate, nameof(StartDate));
    }

    partial void OnStartDateChanged(string? value)
    {
        ValidateProperty(EndDate, nameof(EndDate));
    }

    private bool CanSearch()
    {
        return SourceSelected;
    }

    [RelayCommand(CanExecute = nameof(CanSearch))]
    private async Task OnSearch()
    {
        if (!Sources.Any(s => s.IsSelected))
        {
            return;
        }

        var query = new SearchSourcesQuery
        {
            PageNumber = 1,
            PageSize = 20
        };
        foreach (var source in Sources.Where(s => s.IsSelected))
        {
            var sourceFilters = new SearchSourcesQuery.Source(source.SourceId);
            foreach (var formGroup in FormGroups.Where(f => f.SourceId == source.SourceId))
            {
                foreach (var formElement in formGroup.Elements)
                {
                    switch (formElement.Type)
                    {
                        case FormElementType.MultiSelect when formElement.SelectedOptions.Count > 0:
                            sourceFilters.SelectedFiltersValues.Add($"sad.{formElement.Label}", formElement.SelectedOptions.ToList());
                            break;
                        case FormElementType.CheckBox when !string.IsNullOrEmpty(formElement.TextValue):
                            break;
                        case FormElementType.TextBox when !string.IsNullOrEmpty(formElement.TextValue):
                            sourceFilters.SelectedFiltersValues.Add($"sad.{formElement.Label}", [formElement.TextValue]);
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(StartDate))
                {
                    sourceFilters.SelectedFiltersValues.Add("startDate", [StartDate]);
                }

                if (!string.IsNullOrEmpty(EndDate))
                {
                    sourceFilters.SelectedFiltersValues.Add("endDate", [EndDate]);
                }

                if (!string.IsNullOrEmpty(SearchPhrase))
                {
                    sourceFilters.SelectedFiltersValues.Add("search", [SearchPhrase]);
                }
            }

            query.Sources.Add(sourceFilters);
        }

        var searchResponse = await _mediator.Send(query);

        Columns.Clear();
        Results.Clear();

        if (searchResponse.IsSuccess && searchResponse.SourceDataItems.Any())
        {
            Columns.Add(new ResultColumnDefinition("Source", $"SourceName"));
            Columns.Add(new ResultColumnDefinition("Title", $"Title"));
            Columns.Add(new ResultColumnDefinition("Start Date", $"StartDate"));
            Columns.Add(new ResultColumnDefinition("End Date", $"EndDate"));
            OnUpdateColumns();

            foreach (var sourceDataItem in searchResponse.SourceDataItems)
            {
                var singleSourceResults = new Dictionary<string, object>();
                if (sourceDataItem.Title is null)
                {
                    continue;
                }

                singleSourceResults.Add("SourceName", sourceDataItem.SourceId);
                singleSourceResults.Add("Title", sourceDataItem.Title);
                singleSourceResults.Add("StartDate", sourceDataItem.StartDate.ToString("yyyy-MM-dd"));
                singleSourceResults.Add("EndDate", sourceDataItem.EndDate.ToString("yyyy-MM-dd"));

                // foreach (var additionalData in sourceDataItem.AdditionalData)
                // {
                //     var resultRow = new Dictionary<string, object>();
                //
                //     foreach (var property in sourceDataItem.AdditionalData)
                //     {
                //         if (property.Value != null)
                //         {
                //             resultRow[property.Key] = property.Value;
                //         }
                //     }
                //
                //     Results.Add(resultRow);
                // }
                Results.Add(singleSourceResults);
            }
        }
    }

    private void OnUpdateColumns()
    {
        _columnSubject.OnNext(Columns.ToList());
    }

    private void CalendarsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ;
    }

    private void SourcesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        LoadFormGroups();
        SourceSelected = Sources.Any(s => s.IsSelected);
    }

    private void LoadFormGroups()
    {
        FormGroups.Clear();
        var selectedSources = Sources.Where(x => x.IsSelected).Select(x => x.SourceId).ToList();
        foreach (var formGroup in _formGroups.Where(f => selectedSources.Contains(f.SourceId)))
        {
            FormGroups.Add(formGroup);
        }
    }

    private async Task LoadSourcesAsync()
    {
        var getSourcesForFilteringDto = await _mediator.Send(new GetSourcesForFilteringQuery());
        Sources.Clear();
        _formGroups.Clear();
        foreach (var source in getSourcesForFilteringDto.Sources)
        {
            var sourceViewModel = _sourceViewModelFactory.Create(source);
            Sources.Add(sourceViewModel);
            var filters = _formElementFactory.CreateFormElement(source);
            _formGroups.Add(filters);
        }
    }

    private async Task LoadCalendarsAsync()
    {
        var getCalendarsForFilteringDtos = await _mediator.Send(new GetCalendarsForFilteringQuery());
        Calendars.Clear();
        var calendarViewModels = _calendarViewModelFactory.Create(getCalendarsForFilteringDtos);
        foreach (var calendarViewModel in calendarViewModels)
        {
            Calendars.Add(calendarViewModel);
        }
    }
}
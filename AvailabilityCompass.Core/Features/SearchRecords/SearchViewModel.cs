using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AvailabilityCompass.Core.Features.SearchRecords.FilterFormElements;
using AvailabilityCompass.Core.Features.SearchRecords.Queries.GetAvailableDates;
using AvailabilityCompass.Core.Features.SearchRecords.Queries.GetCalendars;
using AvailabilityCompass.Core.Features.SearchRecords.Queries.GetSources;
using AvailabilityCompass.Core.Features.SearchRecords.Queries.SearchSources;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.EventBus;
using AvailabilityCompass.Core.Shared.ValidationAttributes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;

namespace AvailabilityCompass.Core.Features.SearchRecords;

public partial class SearchViewModel : ObservableValidator, IPageViewModel, IDisposable
{
    private readonly IDisposable _calendarAddedSubscription;
    private readonly ICalendarFilterViewModelFactory _calendarFilterViewModelFactory;
    private readonly BehaviorSubject<List<ResultColumnDefinition>> _columnSubject = new([]);
    private readonly IFormElementFactory _formElementFactory;
    private readonly List<FormGroup> _formGroups = [];
    private readonly IMediator _mediator;
    private readonly ISourceFilterViewModelFactory _sourceFilterViewModelFactory;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [DateRangeValidation(nameof(StartDate))]
    [NotifyPropertyChangedFor(nameof(StartDate))]
    [DateValidation]
    private string? _endDate;

    private bool _initialDataLoaded;

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
        ISourceFilterViewModelFactory sourceFilterViewModelFactory,
        ICalendarFilterViewModelFactory calendarFilterViewModelFactory,
        IFormElementFactory formElementFactory,
        IEventBus eventBus)
    {
        _mediator = mediator;
        _sourceFilterViewModelFactory = sourceFilterViewModelFactory;
        _calendarFilterViewModelFactory = calendarFilterViewModelFactory;
        _formElementFactory = formElementFactory;
        Sources.CollectionChanged += SourcesOnCollectionChanged;

        _calendarAddedSubscription = eventBus.ListenToAll()
            .SelectMany(_ => Observable.FromAsync(OnFilterDataChanged))
            .Subscribe();
    }

    public IObservable<List<ResultColumnDefinition>> ColumnObservable => _columnSubject.AsObservable();

    public FullyObservableCollection<SourceFilterViewModel> Sources { get; } = [];

    public ObservableCollection<CalendarFilterViewModel> Calendars { get; } = [];

    public ObservableCollection<FormGroup> FormGroups { get; set; } = [];

    public ObservableCollection<ResultColumnDefinition> Columns { get; } = [];
    public ObservableCollection<Dictionary<string, object>> Results { get; } = [];

    public void Dispose()
    {
        Sources.CollectionChanged -= SourcesOnCollectionChanged;
        _calendarAddedSubscription.Dispose();
    }

    public bool IsActive { get; set; }

    public string Icon => "SearchWeb";
    public string Name => "Search";

    public async Task LoadDataAsync(CancellationToken ct)
    {
        if (_initialDataLoaded)
        {
            Results.Clear();
            await Task.Delay(100, ct);
            await OnSearch();
            return;
        }

        await LoadCalendarsAsync(ct);
        await LoadSourcesAsync(ct);
        _initialDataLoaded = true;
    }

    private async Task OnFilterDataChanged(CancellationToken ct)
    {
        Results.Clear();
        await LoadCalendarsAsync(ct);
        await LoadSourcesAsync(ct);
    }

    // ReSharper disable once UnusedParameterInPartialMethod
    partial void OnEndDateChanged(string? value)
    {
        ValidateProperty(StartDate, nameof(StartDate));
    }

    // ReSharper disable once UnusedParameterInPartialMethod
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

        var selectedCalendars = Calendars.Where(c => c.IsSelected).Select(c => c.Id).ToList();
        var availableDatesResponse = await _mediator.Send(new GetAvailableDatesQuery(selectedCalendars));

        var query = new SearchSourcesQuery
        {
            ReservedDates = availableDatesResponse.ReservedDates
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
            Columns.Add(new ResultColumnDefinition("Source", "SourceName"));
            Columns.Add(new ResultColumnDefinition("Title", "Title"));
            Columns.Add(new ResultColumnDefinition("Start Date", "StartDate"));
            Columns.Add(new ResultColumnDefinition("End Date", "EndDate"));

            foreach (var sourceDataItem in searchResponse.SourceDataItems)
            {
                var singleSourceResults = new Dictionary<string, object>();
                if (sourceDataItem.Title is null)
                {
                    continue;
                }

                singleSourceResults.Add("SourceName", sourceDataItem.SourceId);
                singleSourceResults.Add("Title", sourceDataItem.Title);
                singleSourceResults.Add("Url", sourceDataItem.Url ?? string.Empty);
                singleSourceResults.Add("StartDate", sourceDataItem.StartDate.ToString("yyyy-MM-dd"));
                singleSourceResults.Add("EndDate", sourceDataItem.EndDate.ToString("yyyy-MM-dd"));

                foreach (var (key, value) in sourceDataItem.AdditionalData)
                {
                    if (Columns.All(columnDefinition => columnDefinition.PropertyName != key))
                    {
                        Columns.Add(new ResultColumnDefinition(key, key));
                    }

                    singleSourceResults.Add(key, value ?? string.Empty);
                }

                Results.Add(singleSourceResults);
            }

            Columns.Add(new ResultColumnDefinition("URL", "Url"));
            OnUpdateColumns();
        }
    }

    private void OnUpdateColumns()
    {
        _columnSubject.OnNext(Columns.ToList());
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

    private async Task LoadSourcesAsync(CancellationToken ct)
    {
        var getSourcesForFilteringDto = await _mediator.Send(new GetSourcesForFilteringQuery(), ct);
        Sources.Clear();
        _formGroups.Clear();
        foreach (var source in getSourcesForFilteringDto.Sources)
        {
            var sourceViewModel = _sourceFilterViewModelFactory.Create(source);
            Sources.Add(sourceViewModel);
            var filters = _formElementFactory.CreateFormElement(source);
            _formGroups.Add(filters);
        }
    }

    private async Task LoadCalendarsAsync(CancellationToken ct)
    {
        var getCalendarsForFilteringResponses = await _mediator.Send(new GetCalendarsForFilteringQuery(), ct);
        Calendars.Clear();
        var calendarViewModels = _calendarFilterViewModelFactory.Create(getCalendarsForFilteringResponses.Calendars);
        foreach (var calendarViewModel in calendarViewModels)
        {
            Calendars.Add(calendarViewModel);
        }
    }
}
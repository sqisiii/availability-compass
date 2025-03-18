using System.Collections.ObjectModel;
using System.Collections.Specialized;
using AvailabilityCompass.Core.Features.Search.FilterFormElements;
using AvailabilityCompass.Core.Features.Search.Queries.GetCalendars;
using AvailabilityCompass.Core.Features.Search.Queries.GetSources;
using AvailabilityCompass.Core.Shared;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;

namespace AvailabilityCompass.Core.Features.Search;

public partial class SearchViewModel : ObservableValidator, IPageViewModel, IDisposable
{
    private readonly ICalendarViewModelFactory _calendarViewModelFactory;
    private readonly IFormElementFactory _formElementFactory;

    private readonly List<FormGroup> _formGroups = [];
    private readonly IMediator _mediator;
    private readonly ISourceViewModelFactory _sourceViewModelFactory;

    [ObservableProperty]
    private DateOnly? _endDate;

    private bool _isActive;

    [ObservableProperty]
    private string? _searchPhrase;

    [ObservableProperty]
    private object? _selectedResult;

    [ObservableProperty]
    private DateOnly? _startDate;

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

    [RelayCommand]
    private void OnSearch()
    {
    }

    private void CalendarsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        ;
    }

    private void SourcesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        LoadFormGroups();
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

    private void UpdateDataGridStructure()
    {
        Columns.Clear();

        Columns.Add(new ResultColumnDefinition("ID", "ID"));
        Columns.Add(new ResultColumnDefinition("Name", "Name"));
        Columns.Add(new ResultColumnDefinition("Score", "Score"));
        Columns.Add(new ResultColumnDefinition("Rank", "Rank"));

        Results.Add(new Dictionary<string, object> { { "ID", 1 }, { "Name", "Alice" }, { "Score", 95 }, { "Rank", "A" } });
        Results.Add(new Dictionary<string, object> { { "ID", 2 }, { "Name", "Bob" }, { "Score", 88 }, { "Rank", "B" } });
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
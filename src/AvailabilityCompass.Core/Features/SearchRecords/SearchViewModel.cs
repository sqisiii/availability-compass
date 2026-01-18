using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using AvailabilityCompass.Core.Features.SearchRecords.FilterFormElements;
using AvailabilityCompass.Core.Features.SearchRecords.Queries.GetCalendars;
using AvailabilityCompass.Core.Features.SearchRecords.Queries.GetSources;
using AvailabilityCompass.Core.Features.SearchRecords.Search;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.EventBus;
using AvailabilityCompass.Core.Shared.ValidationAttributes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;

namespace AvailabilityCompass.Core.Features.SearchRecords;

/// <summary>
/// ViewModel for the search functionality, managing filters, sources, calendars, and search results.
/// </summary>
public sealed partial class SearchViewModel : ObservableValidator, IPageViewModel, IDisposable
{
    private const string NoneSelectedDefault = "None selected";
    private readonly IDisposable _calendarAddedSubscription;
    private readonly ICalendarFilterViewModelFactory _calendarFilterViewModelFactory;
    private readonly BehaviorSubject<List<ResultColumnDefinition>> _columnSubject = new([]);
    private readonly IFormElementFactory _formElementFactory;
    private readonly List<FormGroup> _formGroups = [];
    private readonly IMediator _mediator;
    private readonly ISearchCommandFactory _searchCommandFactory;
    private readonly ISourceFilterViewModelFactory _sourceFilterViewModelFactory;


    [ObservableProperty]
    [NotifyDataErrorInfo]
    [DateRangeValidation(nameof(StartDate))]
    [NotifyPropertyChangedFor(nameof(StartDate))]
    [NotifyPropertyChangedFor(nameof(FiltersSummary))]
    [NotifyPropertyChangedFor(nameof(HasFiltersSet))]
    [DateValidation]
    private string? _endDate;

    private bool _initialDataLoaded;

    [ObservableProperty]
    private bool _isCalendarsSectionExpanded;

    [ObservableProperty]
    private bool _isFiltersSectionExpanded;

    [ObservableProperty]
    private bool _isSourcesSectionExpanded;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FiltersSummary))]
    [NotifyPropertyChangedFor(nameof(HasFiltersSet))]
    private string? _searchPhrase;

    [ObservableProperty]
    private object? _selectedResult;

    [ObservableProperty]
    private string _selectedSortOption = "Date";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SearchCommand))]
    private bool _sourceSelected;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [DateValidation]
    [DateRangeValidation(nameof(EndDate), true)]
    [NotifyPropertyChangedFor(nameof(EndDate))]
    [NotifyPropertyChangedFor(nameof(FiltersSummary))]
    [NotifyPropertyChangedFor(nameof(HasFiltersSet))]
    private string? _startDate;


    public SearchViewModel(
        IMediator mediator,
        ISourceFilterViewModelFactory sourceFilterViewModelFactory,
        ICalendarFilterViewModelFactory calendarFilterViewModelFactory,
        IFormElementFactory formElementFactory,
        IEventBus eventBus,
        ISearchCommandFactory searchCommandFactory)
    {
        _mediator = mediator;
        _sourceFilterViewModelFactory = sourceFilterViewModelFactory;
        _calendarFilterViewModelFactory = calendarFilterViewModelFactory;
        _formElementFactory = formElementFactory;
        _searchCommandFactory = searchCommandFactory;
        Sources.CollectionChanged += SourcesOnCollectionChanged;
        Calendars.CollectionChanged += CalendarsOnCollectionChanged;
        Results.CollectionChanged += (_, _) => OnPropertyChanged(nameof(HasResults));

        _calendarAddedSubscription = eventBus.ListenToAll()
            .SelectMany(_ => Observable.FromAsync(OnFilterDataChanged))
            .Subscribe();
    }

    public bool SourcesAvailable => Sources.Any(s => s.ChangeAt > DateTime.MinValue);
    public bool SourcesUnAvailable => !SourcesAvailable;

    public bool CalendarsAvailable => Calendars.Count > 0;

    public bool CalendarsUnAvailable => !CalendarsAvailable;

    public IObservable<List<ResultColumnDefinition>> ColumnObservable => _columnSubject.AsObservable();

    public FullyObservableCollection<SourceFilterViewModel> Sources { get; } = [];

    public FullyObservableCollection<CalendarFilterViewModel> Calendars { get; } = [];

    public ObservableCollection<FormGroup> FormGroups { get; } = [];

    public List<ResultColumnDefinition> Columns { get; } = [];
    public RangeObservableCollection<Dictionary<string, object>> Results { get; } = [];
    public List<string> SortOptions { get; } = ["Date", "Source", "Title"];

    public bool HasResults => Results.Count > 0;

    public string CalendarsSummary => GetCalendarsSummary();
    public string SourcesSummary => GetSourcesSummary();
    public string FiltersSummary => GetFiltersSummary();

    public bool HasCalendarsSelected => Calendars.Any(c => c.IsSelected);
    public bool HasAvailableDaysSelected => Calendars.Any(c => c.IsSelected && c.IsOnly);
    public bool HasBlockedDaysSelected => Calendars.Any(c => c.IsSelected && !c.IsOnly);
    public string AvailableDaysSummary => GetCalendarsSummaryByType(isOnly: true);
    public string BlockedDaysSummary => GetCalendarsSummaryByType(isOnly: false);
    public bool HasSourcesSelected => Sources.Any(s => s.IsSelected);
    public bool HasFiltersSet => !string.IsNullOrEmpty(SearchPhrase) || !string.IsNullOrEmpty(StartDate) || !string.IsNullOrEmpty(EndDate);

    public void Dispose()
    {
        Sources.CollectionChanged -= SourcesOnCollectionChanged;
        Calendars.CollectionChanged -= CalendarsOnCollectionChanged;
        UnsubscribeFromFormElements();
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

    partial void OnIsCalendarsSectionExpandedChanged(bool value)
        => CollapseOtherSections(value, nameof(IsCalendarsSectionExpanded));

    partial void OnIsSourcesSectionExpandedChanged(bool value)
        => CollapseOtherSections(value, nameof(IsSourcesSectionExpanded));

    partial void OnIsFiltersSectionExpandedChanged(bool value)
        => CollapseOtherSections(value, nameof(IsFiltersSectionExpanded));

    private void CollapseOtherSections(bool isExpanding, string expandedSection)
    {
        if (!isExpanding) return;

        if (expandedSection != nameof(IsCalendarsSectionExpanded))
            IsCalendarsSectionExpanded = false;
        if (expandedSection != nameof(IsSourcesSectionExpanded))
            IsSourcesSectionExpanded = false;
        if (expandedSection != nameof(IsFiltersSectionExpanded))
            IsFiltersSectionExpanded = false;
    }

    // ReSharper disable once UnusedParameterInPartialMethod
    partial void OnSelectedSortOptionChanged(string value)
    {
        _ = SortResultsAsync();
    }

    private async Task SortResultsAsync()
    {
        if (Results.Count == 0) return;

        // Capture current items for sorting on a background thread
        var itemsToSort = Results.ToList();
        var sortOption = SelectedSortOption;

        // Sort on background thread to avoid blocking UI
        var sorted = await Task.Run(() => sortOption switch
        {
            "Date" => itemsToSort.OrderBy(r => r.TryGetValue("StartDate", out var d) ? d.ToString() ?? "" : "").ToList(),
            "Source" => itemsToSort.OrderBy(r => r.TryGetValue("SourceName", out var s) ? s.ToString() ?? "" : "").ToList(),
            "Title" => itemsToSort.OrderBy(r => r.TryGetValue("Title", out var t) ? t.ToString() ?? "" : "").ToList(),
            _ => itemsToSort
        });

        // Single batch update with one CollectionChanged notification
        Results.ReplaceAll(sorted);
    }

    private bool CanSearch()
    {
        return SourceSelected;
    }

    [RelayCommand(CanExecute = nameof(CanSearch))]
    private async Task OnSearch()
    {
        IsCalendarsSectionExpanded = false;
        IsSourcesSectionExpanded = false;
        IsFiltersSectionExpanded = false;

        await _searchCommandFactory.Create().ExecuteAsync();
    }

    public void OnUpdateColumns()
    {
        _columnSubject.OnNext(Columns.ToList());
    }

    private void SourcesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        LoadFormGroups();
        SourceSelected = Sources.Any(s => s.IsSelected);
        OnPropertyChanged(nameof(SourcesSummary));
        OnPropertyChanged(nameof(HasSourcesSelected));
    }

    private void CalendarsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        => NotifyCalendarRelatedPropertiesChanged();

    private void NotifyCalendarRelatedPropertiesChanged()
    {
        OnPropertyChanged(nameof(CalendarsSummary));
        OnPropertyChanged(nameof(HasCalendarsSelected));
        OnPropertyChanged(nameof(HasAvailableDaysSelected));
        OnPropertyChanged(nameof(HasBlockedDaysSelected));
        OnPropertyChanged(nameof(AvailableDaysSummary));
        OnPropertyChanged(nameof(BlockedDaysSummary));
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
        UnsubscribeFromFormElements();

        var getSourcesForFilteringDto = await _mediator.Send(new GetSourcesForFilteringQuery(), ct);
        Sources.Clear();
        _formGroups.Clear();
        foreach (var source in getSourcesForFilteringDto.Sources)
        {
            var sourceViewModel = _sourceFilterViewModelFactory.Create(source);
            var filters = _formElementFactory.CreateFormElement(source);
            _formGroups.Add(filters);
            sourceViewModel.FilterFormGroup = filters;
            Sources.Add(sourceViewModel);
        }

        SubscribeToFormElements();

        OnPropertyChanged(nameof(SourcesAvailable));
        OnPropertyChanged(nameof(SourcesUnAvailable));
    }

    private void SubscribeToFormElements()
    {
        foreach (var formGroup in _formGroups)
        {
            foreach (var element in formGroup.Elements)
            {
                element.PropertyChanged += OnFormElementPropertyChanged;
                element.SelectedOptions.CollectionChanged += OnSelectedOptionsChanged;
            }
        }
    }

    private void UnsubscribeFromFormElements()
    {
        foreach (var formGroup in _formGroups)
        {
            foreach (var element in formGroup.Elements)
            {
                element.PropertyChanged -= OnFormElementPropertyChanged;
                element.SelectedOptions.CollectionChanged -= OnSelectedOptionsChanged;
            }
        }
    }

    private void OnFormElementPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(FormElement.TextValue))
        {
            OnPropertyChanged(nameof(SourcesSummary));
        }
    }

    private void OnSelectedOptionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(SourcesSummary));
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

        OnPropertyChanged(nameof(CalendarsAvailable));
        OnPropertyChanged(nameof(CalendarsUnAvailable));
    }

    private string GetCalendarsSummary()
        => GetSelectionSummary(Calendars, c => c.IsSelected, c => c.Name);

    private string GetCalendarsSummaryByType(bool isOnly)
        => GetSelectionSummary(
            Calendars,
            c => c.IsSelected && c.IsOnly == isOnly,
            c => c.Name,
            maxDisplayCount: 2,
            noneSelectedText: string.Empty);

    private static string GetSelectionSummary<T>(
        IEnumerable<T> items,
        Func<T, bool> isSelectedPredicate,
        Func<T, string> nameSelector,
        int maxDisplayCount = 3,
        string noneSelectedText = NoneSelectedDefault)
    {
        var selectedItems = items.Where(isSelectedPredicate).ToList();
        if (selectedItems.Count == 0)
            return noneSelectedText;

        var names = selectedItems.Select(nameSelector).Take(maxDisplayCount);
        var summary = string.Join(", ", names);

        if (selectedItems.Count > maxDisplayCount)
            summary += $" +{selectedItems.Count - maxDisplayCount} more";

        return summary;
    }

    private string GetSourcesSummary()
    {
        var selectedSources = Sources.Where(s => s.IsSelected).ToList();
        if (selectedSources.Count == 0)
        {
            return "None selected";
        }

        var summaries = selectedSources.Select(s =>
            {
                var filterCount = GetActiveFilterCountForSource(s.SourceId);
                return filterCount > 0 ? $"{s.Name} ({filterCount})" : s.Name;
            })
            .Take(3);

        var summary = string.Join(", ", summaries);
        if (selectedSources.Count > 3)
        {
            summary += $" +{selectedSources.Count - 3} more";
        }

        return summary;
    }

    private int GetActiveFilterCountForSource(string sourceId)
    {
        var formGroup = _formGroups.FirstOrDefault(f => f.SourceId == sourceId);
        if (formGroup == null)
        {
            return 0;
        }

        return formGroup.Elements.Count(e =>
            !string.IsNullOrEmpty(e.TextValue) ||
            e.SelectedOptions.Any());
    }

    private string GetFiltersSummary()
    {
        var parts = new List<string>();

        if (!string.IsNullOrEmpty(SearchPhrase))
        {
            parts.Add($"\"{SearchPhrase}\"");
        }

        if (!string.IsNullOrEmpty(StartDate))
        {
            parts.Add($"From: {StartDate}");
        }

        if (!string.IsNullOrEmpty(EndDate))
        {
            parts.Add($"To: {EndDate}");
        }

        return parts.Count > 0 ? string.Join(" | ", parts) : "No filters";
    }
}
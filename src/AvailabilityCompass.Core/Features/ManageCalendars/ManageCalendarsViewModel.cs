using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Linq;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddCalendarRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddDateEntryRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteCalendarRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteDateEntryRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateCalendarRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateDateEntryRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.DatesCalculator;
using AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetCalendarsQuery;
using AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetDateEntriesQuery;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.EventBus;
using AvailabilityCompass.Core.Shared.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

/// <summary>
/// Represents a detected date selection which may be a single date or a consecutive period.
/// </summary>
public record DetectedSelection(DateOnly StartDate, int Duration)
{
    public DateOnly EndDate => StartDate.AddDays(Duration - 1);
    public bool IsPeriod => Duration > 1;

    public string DisplayText => IsPeriod
        ? $"{StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd} ({Duration} days)"
        : $"{StartDate:yyyy-MM-dd}";
}

/// <summary>
/// ViewModel for managing calendars
/// </summary>
public partial class ManageCalendarsViewModel : ObservableValidator, IPageViewModel, IDialogViewModel, IDisposable
{
    private readonly ICalendarViewModelFactory _calendarViewModelFactory;
    private readonly INavigationService<IDialogViewModel> _dialogNavigationService;
    private readonly IMediator _mediator;
    private readonly IReservedDatesCalculator _reservedDatesCalculator;

    private IDisposable? _calendarAddedSubscription;
    private IDisposable? _calendarDeletedSubscription;
    private IDisposable? _calendarUpdatedSubscription;
    private IDisposable? _dateEntryAddedSubscription;
    private IDisposable? _dateEntryDeletedSubscription;
    private IDisposable? _dateEntryUpdatedSubscription;

    private Guid? _editingEntryId;

    [ObservableProperty]
    private string _editorDescription = string.Empty;

    [ObservableProperty]
    private ObservableCollection<DetectedSelection> _editorDetectedSelections = [];

    [ObservableProperty]
    private int _editorDuration = 1;

    [ObservableProperty]
    private int? _editorFrequency;

    [ObservableProperty]
    private bool _editorIsRecurring;

    [ObservableProperty]
    private int _editorRepetitions;

    [ObservableProperty]
    private string? _editorStartDateString;

    [ObservableProperty]
    private string _editorTitle = "Add Date Entry";

    [ObservableProperty]
    private bool _hasMultipleSelections;

    // Inline add calendar state
    [ObservableProperty]
    private bool _isAddCalendarExpanded;

    [ObservableProperty]
    private bool _isDialogOpen;

    [ObservableProperty]
    private bool _isEditMode;

    // Inline editor state
    [ObservableProperty]
    private bool _isEditorOpen;

    [ObservableProperty]
    private bool _newCalendarIsOnly;

    [ObservableProperty]
    private string _newCalendarName = string.Empty;

    // For bulk creation - stores parsed selections with period detection
    private List<DetectedSelection>? _pendingSelections;

    [NotifyPropertyChangedFor(nameof(IsCalendarSelected))]
    [NotifyPropertyChangedFor(nameof(CalendarName))]
    [NotifyPropertyChangedFor(nameof(CalendarIsOnly))]
    [ObservableProperty]
    private CalendarViewModel? _selectedCalendar;

    public ManageCalendarsViewModel(
        IMediator mediator,
        IEventBus eventBus,
        INavigationService<IDialogViewModel> dialogNavigationService,
        ICalendarViewModelFactory calendarViewModelFactory,
        IReservedDatesCalculator reservedDatesCalculator)
    {
        _mediator = mediator;
        _dialogNavigationService = dialogNavigationService;
        _calendarViewModelFactory = calendarViewModelFactory;
        _reservedDatesCalculator = reservedDatesCalculator;
        Calendars.CollectionChanged += CalendarsOnCollectionChanged;

        SubscribeToEvents(eventBus);
    }

    public string CalendarName => SelectedCalendar?.Name ?? string.Empty;
    public bool IsCalendarSelected => SelectedCalendar is not null;
    public bool CalendarIsOnly => SelectedCalendar?.IsOnly ?? false;
    public Guid SelectedCalendarId => SelectedCalendar?.CalendarId ?? Guid.Empty;

    public FullyObservableCollection<CalendarViewModel> Calendars { get; } = [];
    public ObservableCollection<DateEntryViewModel> DateEntries { get; } = [];
    public ObservableCollection<CategorizedDate> ReservedDates { get; set; } = [];

    public void Dispose()
    {
        _calendarAddedSubscription?.Dispose();
        _calendarDeletedSubscription?.Dispose();
        _calendarUpdatedSubscription?.Dispose();
        _dateEntryAddedSubscription?.Dispose();
        _dateEntryUpdatedSubscription?.Dispose();
        _dateEntryDeletedSubscription?.Dispose();
    }

    public bool IsActive { get; set; }
    public string Icon => "CalendarClock";
    public string Name => "Calendars";

    public async Task LoadDataAsync(CancellationToken ct)
    {
        await LoadCalendars(ct);
    }

    private void SubscribeToEvents(IEventBus eventBus)
    {
        _calendarAddedSubscription = eventBus.Listen<CalendarAddedEvent>()
            .SelectMany(evt => Observable.FromAsync(ct => OnCalendarAdded(evt, ct)))
            .Subscribe();
        _calendarDeletedSubscription = eventBus.Listen<CalendarDeletedEvent>()
            .SelectMany(_ => Observable.FromAsync(OnCalendarDeleted))
            .Subscribe();
        _calendarUpdatedSubscription = eventBus.Listen<CalendarUpdatedEvent>()
            .SelectMany(_ => Observable.FromAsync(OnCalendarUpdated))
            .Subscribe();
        _dateEntryAddedSubscription = eventBus.Listen<DateEntryAddedEvent>()
            .SelectMany(evt => Observable.FromAsync(ct => OnDateEntryChanged(evt.CalendarId, ct)))
            .Subscribe();
        _dateEntryDeletedSubscription = eventBus.Listen<DateEntryDeletedEvent>()
            .SelectMany(evt => Observable.FromAsync(ct => OnDateEntryChanged(evt.CalendarId, ct)))
            .Subscribe();
        _dateEntryUpdatedSubscription = eventBus.Listen<DateEntryUpdatedEvent>()
            .SelectMany(evt => Observable.FromAsync(ct => OnDateEntryChanged(evt.CalendarId, ct)))
            .Subscribe();
    }

    private async Task OnCalendarAdded(CalendarAddedEvent evt, CancellationToken ct)
    {
        await LoadCalendars(evt.CalendarId, ct);
    }

    private async Task OnCalendarDeleted(CancellationToken ct)
    {
        await LoadCalendars(ct);
    }

    private async Task OnCalendarUpdated(CancellationToken ct)
    {
        await LoadCalendars(ct);
    }

    private async Task OnDateEntryChanged(Guid calendarId, CancellationToken ct)
    {
        await RefreshDateEntriesAsync(calendarId, ct);
        CalculateReservedDays();
    }

    private async Task RefreshDateEntriesAsync(Guid calendarId, CancellationToken ct)
    {
        var response = await _mediator.Send(new GetDateEntriesQuery(calendarId), ct);
        if (response.IsSuccess)
        {
            DateEntries.Clear();
            SelectedCalendar?.DateEntries.Clear();
            foreach (var dateEntryDto in response.DateEntries)
            {
                var dateEntry = _calendarViewModelFactory.CreateDateEntry(dateEntryDto);
                DateEntries.Add(dateEntry);
                SelectedCalendar?.DateEntries.Add(dateEntry);
            }
        }
    }

    private void CalendarsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SelectedCalendar = Calendars.FirstOrDefault(x => x.IsSelected);
        if (SelectedCalendar is null)
        {
            return;
        }

        DateEntries.Clear();
        foreach (var dateEntry in SelectedCalendar.DateEntries)
        {
            DateEntries.Add(dateEntry);
        }

        CalculateReservedDays();
    }

    private async Task LoadCalendars(CancellationToken ct)
    {
        await LoadCalendars(null, ct);
    }

    private async Task LoadCalendars(Guid? newSelectedCalendarId, CancellationToken ct)
    {
        var previouslySelectedCalendarId = SelectedCalendar?.CalendarId;
        Calendars.Clear();
        var calendarResponse = await _mediator.Send(new GetCalendarsQuery(), ct);
        if (calendarResponse.Calendars is null)
        {
            return;
        }

        foreach (var calendar in calendarResponse.Calendars)
        {
            Calendars.Add(_calendarViewModelFactory.CreateCalendar(calendar));
        }

        var calendarId = newSelectedCalendarId ?? previouslySelectedCalendarId;
        if (calendarId is not null)
        {
            var selectedCalendar = Calendars.FirstOrDefault(x => x.CalendarId == calendarId);
            if (selectedCalendar is not null)
            {
                selectedCalendar.IsSelected = true;
            }
        }
    }

    private void CalculateReservedDays()
    {
        ReservedDates.Clear();
        var reservedDates = _reservedDatesCalculator.GetReservedCategorizedDays(SelectedCalendar);
        foreach (var reservedDate in reservedDates)
        {
            ReservedDates.Add(reservedDate);
        }
    }

    [RelayCommand]
    private async Task OnAddCalendarInline()
    {
        if (string.IsNullOrWhiteSpace(NewCalendarName))
        {
            return;
        }

        await _mediator.Send(new AddCalendarToDbRequest(NewCalendarName, NewCalendarIsOnly));

        IsAddCalendarExpanded = false;
        NewCalendarName = string.Empty;
        NewCalendarIsOnly = false;
    }

    [RelayCommand]
    private void OnCancelAddCalendar()
    {
        IsAddCalendarExpanded = false;
        NewCalendarName = string.Empty;
        NewCalendarIsOnly = false;
    }

    [RelayCommand]
    private void OnExpandAddCalendar()
    {
        IsAddCalendarExpanded = true;
    }

    [RelayCommand]
    private void OnAddSelectedDates(object? selectedDatesObj)
    {
        if (SelectedCalendar is null)
        {
            return;
        }

        // WPF Calendar.SelectedDates is a SelectedDatesCollection that implements IList
        if (selectedDatesObj is not IList selectedDatesList || selectedDatesList.Count == 0)
        {
            return;
        }

        var dates = selectedDatesList.Cast<DateTime>().ToList();
        _pendingSelections = ParseSelectedDates(dates);

        EditorDetectedSelections.Clear();
        foreach (var selection in _pendingSelections)
        {
            EditorDetectedSelections.Add(selection);
        }

        HasMultipleSelections = _pendingSelections.Count > 1;

        EditorTitle = _pendingSelections.Count > 1
            ? $"Add {_pendingSelections.Count} Date Entries"
            : _pendingSelections[0].IsPeriod
                ? "Add Period"
                : "Add Date Entry";

        EditorStartDateString = _pendingSelections[0].StartDate.ToString("yyyy-MM-dd");
        EditorDescription = string.Empty;
        EditorIsRecurring = false;
        EditorDuration = _pendingSelections[0].Duration;
        EditorFrequency = null;
        EditorRepetitions = 0;
        IsEditMode = false;
        _editingEntryId = null;

        IsEditorOpen = true;
    }

    private static List<DetectedSelection> ParseSelectedDates(IEnumerable<DateTime> selectedDates)
    {
        var sortedDates = selectedDates.Select(d => DateOnly.FromDateTime(d)).OrderBy(d => d).ToList();
        var selections = new List<DetectedSelection>();

        if (sortedDates.Count == 0) return selections;

        var currentStart = sortedDates[0];
        var currentDuration = 1;

        for (var i = 1; i < sortedDates.Count; i++)
        {
            var expected = sortedDates[i - 1].AddDays(1);
            if (sortedDates[i] == expected)
            {
                // Consecutive - extend current period
                currentDuration++;
            }
            else
            {
                // Gap found - save current period, start new one
                selections.Add(new DetectedSelection(currentStart, currentDuration));
                currentStart = sortedDates[i];
                currentDuration = 1;
            }
        }

        // Add a final period
        selections.Add(new DetectedSelection(currentStart, currentDuration));
        return selections;
    }

    [RelayCommand]
    private void OnDateClicked(DateTime date)
    {
        if (SelectedCalendar is null)
        {
            return;
        }

        var dateOnly = DateOnly.FromDateTime(date);
        var existingEntry = FindEntryByDate(dateOnly);

        if (existingEntry is not null)
        {
            LoadEntryForEdit(existingEntry);
        }
        else
        {
            PrepareNewEntry(dateOnly);
        }

        IsEditorOpen = true;
    }

    [RelayCommand]
    private void OnEditEntry(DateEntryViewModel entry)
    {
        LoadEntryForEdit(entry);
        IsEditorOpen = true;
    }

    [RelayCommand]
    private async Task OnSaveEntry()
    {
        if (SelectedCalendar is null)
        {
            return;
        }

        // Handle bulk creation from multi-select with period detection
        if (_pendingSelections is { Count: > 0 })
        {
            foreach (var selection in _pendingSelections)
            {
                await _mediator.Send(new AddDateEntryToDbRequest(
                    SelectedCalendar.CalendarId,
                    EditorDescription,
                    selection.StartDate,
                    EditorIsRecurring,
                    selection.Duration,
                    EditorIsRecurring ? EditorFrequency : null,
                    EditorIsRecurring ? EditorRepetitions : 0));
            }

            _pendingSelections = null;
            CloseEditor();
            return;
        }

        // Single date creation/update
        if (string.IsNullOrWhiteSpace(EditorStartDateString))
        {
            return;
        }

        if (!DateOnly.TryParse(EditorStartDateString, out var startDate))
        {
            return;
        }

        if (IsEditMode && _editingEntryId.HasValue)
        {
            await _mediator.Send(new UpdateDateEntryInDbRequest(
                SelectedCalendar.CalendarId,
                _editingEntryId.Value,
                EditorDescription,
                startDate,
                EditorIsRecurring,
                EditorIsRecurring ? EditorDuration : 1,
                EditorIsRecurring ? EditorFrequency : null,
                EditorIsRecurring ? EditorRepetitions : 0));
        }
        else
        {
            await _mediator.Send(new AddDateEntryToDbRequest(
                SelectedCalendar.CalendarId,
                EditorDescription,
                startDate,
                EditorIsRecurring,
                EditorIsRecurring ? EditorDuration : 1,
                EditorIsRecurring ? EditorFrequency : null,
                EditorIsRecurring ? EditorRepetitions : 0));
        }

        CloseEditor();
    }

    [RelayCommand]
    private void OnCancelEdit()
    {
        CloseEditor();
    }

    [RelayCommand]
    private async Task OnDeleteEntry()
    {
        if (!IsEditMode || !_editingEntryId.HasValue || SelectedCalendar is null)
        {
            return;
        }

        await _mediator.Send(new DeleteDateEntryFromDbRequest(
            SelectedCalendar.CalendarId,
            _editingEntryId.Value));

        CloseEditor();
    }

    private DateEntryViewModel? FindEntryByDate(DateOnly date)
    {
        // Check if any entry starts on this date
        return DateEntries.FirstOrDefault(e => e.StartDate == date);
    }

    private void LoadEntryForEdit(DateEntryViewModel entry)
    {
        IsEditMode = true;
        _editingEntryId = entry.DateEntryId;
        _pendingSelections = null;

        EditorDetectedSelections.Clear();
        if (entry.StartDate is { } startDate)
        {
            EditorDetectedSelections.Add(new DetectedSelection(startDate, entry.Duration));
        }

        HasMultipleSelections = false;

        EditorTitle = entry.Duration > 1 ? "Edit Period" : "Edit Date Entry";
        EditorDescription = entry.Description;
        EditorStartDateString = entry.StartDateString;
        EditorIsRecurring = entry.IsRecurring;
        EditorDuration = entry.Duration;
        EditorFrequency = entry.Frequency;
        EditorRepetitions = entry.NumberOfRepetitions;
    }

    private void PrepareNewEntry(DateOnly date)
    {
        IsEditMode = false;
        _editingEntryId = null;
        _pendingSelections = null;

        EditorDetectedSelections.Clear();
        EditorDetectedSelections.Add(new DetectedSelection(date, 1));
        HasMultipleSelections = false;

        EditorTitle = "Add Date Entry";
        EditorDescription = string.Empty;
        EditorStartDateString = date.ToString("yyyy-MM-dd");
        EditorIsRecurring = false;
        EditorDuration = 1;
        EditorFrequency = null;
        EditorRepetitions = 0;
    }

    private void CloseEditor()
    {
        IsEditorOpen = false;
        _editingEntryId = null;
        _pendingSelections = null;
        EditorDetectedSelections.Clear();
        HasMultipleSelections = false;
        EditorDescription = string.Empty;
        EditorStartDateString = null;
        EditorIsRecurring = false;
        EditorDuration = 1;
        EditorFrequency = null;
        EditorRepetitions = 0;
    }

    [RelayCommand]
    private void OnClose()
    {
        IsDialogOpen = false;
        _dialogNavigationService.CloseView();
    }
}
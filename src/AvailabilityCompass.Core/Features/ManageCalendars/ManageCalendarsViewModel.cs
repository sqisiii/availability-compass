using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
/// ViewModel for managing calendars
/// </summary>
public sealed partial class ManageCalendarsViewModel : ObservableValidator, IPageViewModel, IDialogViewModel, IDisposable
{
    private readonly ICalendarCrudController _calendarCrud;
    private readonly ICalendarViewModelFactory _calendarViewModelFactory;
    private readonly IDateEntryEditorController _dateEntryEditor;
    private readonly INavigationService<IDialogViewModel> _dialogNavigationService;
    private readonly IMediator _mediator;
    private readonly IReservedDatesCalculator _reservedDatesCalculator;

    private IDisposable? _calendarAddedSubscription;
    private IDisposable? _calendarDeletedSubscription;
    private IDisposable? _calendarUpdatedSubscription;
    private IDisposable? _dateEntryAddedSubscription;
    private IDisposable? _dateEntryDeletedSubscription;
    private IDisposable? _dateEntryUpdatedSubscription;

    [ObservableProperty]
    private bool _hasSelectedDates;

    [ObservableProperty]
    private bool _isDialogOpen;

    [NotifyPropertyChangedFor(nameof(IsCalendarSelected))]
    [NotifyPropertyChangedFor(nameof(CalendarName))]
    [NotifyPropertyChangedFor(nameof(CalendarIsOnly))]
    [NotifyPropertyChangedFor(nameof(SelectedCalendarId))]
    [ObservableProperty]
    private CalendarViewModel? _selectedCalendar;

    [ObservableProperty]
    private IList? _selectedDates;

    public ManageCalendarsViewModel(
        IMediator mediator,
        IEventBus eventBus,
        INavigationService<IDialogViewModel> dialogNavigationService,
        ICalendarViewModelFactory calendarViewModelFactory,
        IReservedDatesCalculator reservedDatesCalculator,
        ICalendarCrudController calendarCrud,
        IDateEntryEditorController dateEntryEditor)
    {
        _mediator = mediator;
        _dialogNavigationService = dialogNavigationService;
        _calendarViewModelFactory = calendarViewModelFactory;
        _reservedDatesCalculator = reservedDatesCalculator;
        _calendarCrud = calendarCrud;
        _dateEntryEditor = dateEntryEditor;

        Calendars.CollectionChanged += CalendarsOnCollectionChanged;

        // Forward property changes from controllers
        _calendarCrud.PropertyChanged += OnCalendarCrudPropertyChanged;
        _dateEntryEditor.PropertyChanged += OnDateEntryEditorPropertyChanged;

        SubscribeToEvents(eventBus);
    }


    public string CalendarName => SelectedCalendar?.Name ?? string.Empty;
    public bool IsCalendarSelected => SelectedCalendar is not null;
    public bool CalendarIsOnly => SelectedCalendar?.IsOnly ?? false;
    public Guid SelectedCalendarId => SelectedCalendar?.CalendarId ?? Guid.Empty;

    public bool IsAddCalendarExpanded => _calendarCrud.IsAddCalendarExpanded;

    public string NewCalendarName
    {
        get => _calendarCrud.NewCalendarName;
        set => _calendarCrud.NewCalendarName = value;
    }

    public bool NewCalendarIsOnly
    {
        get => _calendarCrud.NewCalendarIsOnly;
        set => _calendarCrud.NewCalendarIsOnly = value;
    }

    public bool IsEditCalendarExpanded => _calendarCrud.IsEditCalendarExpanded;

    public string EditCalendarName
    {
        get => _calendarCrud.EditCalendarName;
        set => _calendarCrud.EditCalendarName = value;
    }

    public bool EditCalendarIsOnly
    {
        get => _calendarCrud.EditCalendarIsOnly;
        set => _calendarCrud.EditCalendarIsOnly = value;
    }

    public bool IsDeleteConfirmationOpen => _calendarCrud.IsDeleteConfirmationOpen;
    public string DeleteCalendarName => _calendarCrud.DeleteCalendarName;


    public bool IsEditorOpen => _dateEntryEditor.IsEditorOpen;
    public bool IsEditMode => _dateEntryEditor.IsEditMode;
    public string EditorTitle => _dateEntryEditor.EditorTitle;

    public string EditorDescription
    {
        get => _dateEntryEditor.EditorDescription;
        set => _dateEntryEditor.EditorDescription = value;
    }

    public bool EditorIsRecurring
    {
        get => _dateEntryEditor.EditorIsRecurring;
        set => _dateEntryEditor.EditorIsRecurring = value;
    }

    public int? EditorFrequency
    {
        get => _dateEntryEditor.EditorFrequency;
        set => _dateEntryEditor.EditorFrequency = value;
    }

    public int EditorRepetitions
    {
        get => _dateEntryEditor.EditorRepetitions;
        set => _dateEntryEditor.EditorRepetitions = value;
    }

    public ObservableCollection<DetectedSelection> EditorDetectedSelections => _dateEntryEditor.EditorDetectedSelections;


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

        _calendarCrud.PropertyChanged -= OnCalendarCrudPropertyChanged;
        _dateEntryEditor.PropertyChanged -= OnDateEntryEditorPropertyChanged;
    }


    public bool IsActive { get; set; }
    public string Icon => "CalendarClock";
    public string Name => "Calendars";

    public async Task LoadDataAsync(CancellationToken ct)
    {
        await LoadCalendars(ct);
    }


    [RelayCommand]
    private void OnClose()
    {
        IsDialogOpen = false;
        _dialogNavigationService.CloseView();
    }


    [RelayCommand]
    private Task OnAddCalendarInline() => _calendarCrud.AddCalendarAsync();

    [RelayCommand]
    private void OnCancelAddCalendar() => _calendarCrud.CancelAddCalendar();

    [RelayCommand]
    private void OnExpandAddCalendar()
    {
        _calendarCrud.ExpandAddCalendar(() =>
        {
            if (SelectedCalendar is null)
            {
                return;
            }

            SelectedCalendar.IsSelected = false;
            SelectedCalendar = null;
        });
    }

    [RelayCommand]
    private void OnUpdateCalendar(Guid calendarId)
    {
        var calendar = Calendars.FirstOrDefault(c => c.CalendarId == calendarId);
        if (calendar is not null)
        {
            _calendarCrud.StartCalendarEdit(calendar);
        }
    }

    [RelayCommand]
    private Task OnSaveCalendarEdit() => _calendarCrud.SaveCalendarEditAsync();

    [RelayCommand]
    private void OnCancelCalendarEdit() => _calendarCrud.CancelCalendarEdit();

    [RelayCommand]
    private void OnDeleteCalendar(Guid calendarId)
    {
        var calendar = Calendars.FirstOrDefault(c => c.CalendarId == calendarId);
        if (calendar is not null)
        {
            _calendarCrud.StartDeleteCalendar(calendar);
        }
    }

    [RelayCommand]
    private Task OnConfirmDeleteCalendar() => _calendarCrud.ConfirmDeleteCalendarAsync();

    [RelayCommand]
    private void OnCancelDeleteCalendar() => _calendarCrud.CancelDeleteCalendar();

    [RelayCommand]
    private void OnAddSelectedDates(object? selectedDatesObj)
    {
        if (SelectedCalendar is null)
        {
            return;
        }

        _dateEntryEditor.OpenForSelectedDates(selectedDatesObj as IList);
    }

    [RelayCommand]
    private void OnDateClicked(DateTime date)
    {
        if (SelectedCalendar is null)
        {
            return;
        }

        _dateEntryEditor.OpenForDateClick(DateOnly.FromDateTime(date), DateEntries);
    }

    [RelayCommand]
    private void OnEditEntry(DateEntryViewModel entry)
    {
        _dateEntryEditor.OpenForEdit(entry);
    }

    [RelayCommand]
    private async Task OnSaveEntry()
    {
        if (SelectedCalendar is null)
        {
            return;
        }

        await _dateEntryEditor.SaveAsync(SelectedCalendar.CalendarId);
    }

    [RelayCommand]
    private void OnCancelEdit() => _dateEntryEditor.Close();

    [RelayCommand]
    private async Task OnDeleteEntry()
    {
        if (SelectedCalendar is null)
        {
            return;
        }

        await _dateEntryEditor.DeleteAsync(SelectedCalendar.CalendarId);
    }


    // ReSharper disable once UnusedParameterInPartialMethod
    partial void OnSelectedCalendarChanged(CalendarViewModel? value)
    {
        // Close edit mode when switching calendars
        if (IsEditCalendarExpanded)
        {
            _calendarCrud.CancelCalendarEdit();
        }

        // Close add a calendar form when switching calendars
        if (IsAddCalendarExpanded)
        {
            _calendarCrud.CancelAddCalendar();
        }
    }

    private void OnCalendarCrudPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Forward property change notifications to maintain XAML bindings
        OnPropertyChanged(e.PropertyName);
    }

    private void OnDateEntryEditorPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Forward property change notifications to maintain XAML bindings
        OnPropertyChanged(e.PropertyName);
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

    private void CalculateReservedDays()
    {
        ReservedDates.Clear();
        var reservedDates = _reservedDatesCalculator.GetReservedCategorizedDays(SelectedCalendar);
        foreach (var reservedDate in reservedDates)
        {
            ReservedDates.Add(reservedDate);
        }
    }
}
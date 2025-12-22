using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Linq;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddCalendarRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddRecurringDatesRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddSingleDateRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteCalendarRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteRecurringDateRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteSingleDateRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateCalendarRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateRecurringDateRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateSingleDateRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.DatesCalculator;
using AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetCalendarsQuery;
using AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetRecurringDatesQuery;
using AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetSingleDatesQuery;
using AvailabilityCompass.Core.Shared;
using AvailabilityCompass.Core.Shared.EventBus;
using AvailabilityCompass.Core.Shared.Navigation;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

/// <summary>
/// ViewModel for managing calendars, including single and recurring dates.
/// Handles calendar selection, loading, and CRUD operations.
/// </summary>
public partial class ManageCalendarsViewModel : ObservableValidator, IPageViewModel, IDialogViewModel, IDisposable
{
    private readonly ICalendarDialogViewModelsFactory _calendarDialogViewModelsFactory;
    private readonly ICalendarViewModelFactory _calendarViewModelFactory;
    private readonly INavigationService<IDialogViewModel> _dialogNavigationService;
    private readonly IMediator _mediator;
    private readonly IReservedDatesCalculator _reservedDatesCalculator;
    private IDisposable? _calendarAddedSubscription;
    private IDisposable? _calendarDeletedSubscription;
    private IDisposable? _calendarUpdatedSubscription;
    private IDisposable? _recurringDateAddedSubscription;
    private IDisposable? _recurringDateDeletedSubscription;
    private IDisposable? _recurringDateUpdatedSubscription;

    [ObservableProperty]
    private bool _isDialogOpen;

    [NotifyPropertyChangedFor(nameof(IsCalendarSelected))]
    [NotifyPropertyChangedFor(nameof(CalendarName))]
    [NotifyPropertyChangedFor(nameof(CalendarIsOnly))]
    [ObservableProperty]
    private CalendarViewModel? _selectedCalendar;

    private IDisposable? _singleDateAddedSubscription;
    private IDisposable? _singleDateDeletedSubscription;
    private IDisposable? _singleDateUpdatedSubscription;

    public ManageCalendarsViewModel(
        IMediator mediator,
        IEventBus eventBus,
        INavigationService<IDialogViewModel> dialogNavigationService,
        ICalendarViewModelFactory calendarViewModelFactory,
        IReservedDatesCalculator reservedDatesCalculator,
        ICalendarDialogViewModelsFactory calendarDialogViewModelsFactory)
    {
        _mediator = mediator;
        _dialogNavigationService = dialogNavigationService;
        _calendarViewModelFactory = calendarViewModelFactory;
        _reservedDatesCalculator = reservedDatesCalculator;
        _calendarDialogViewModelsFactory = calendarDialogViewModelsFactory;
        Calendars.CollectionChanged += CalendarsOnCollectionChanged;

        SubscribeToEvents(eventBus);
    }

    public string CalendarName => SelectedCalendar?.Name ?? string.Empty;

    public bool IsCalendarSelected => SelectedCalendar is not null;

    public bool CalendarIsOnly => SelectedCalendar?.IsOnly ?? false;

    public FullyObservableCollection<CalendarViewModel> Calendars { get; } = [];

    public ObservableCollection<RecurringDateViewModel> RecurringDates { get; } = [];

    public ObservableCollection<CategorizedDate> ReservedDates { get; set; } = [];
    public ObservableCollection<SingleDateViewModel> SingleDates { get; } = [];


    public Guid SelectedCalendarId => SelectedCalendar?.CalendarId ?? Guid.Empty;

    public void Dispose()
    {
        _calendarAddedSubscription?.Dispose();
        _calendarDeletedSubscription?.Dispose();
        _calendarUpdatedSubscription?.Dispose();
        _singleDateAddedSubscription?.Dispose();
        _singleDateUpdatedSubscription?.Dispose();
        _singleDateDeletedSubscription?.Dispose();
        _recurringDateAddedSubscription?.Dispose();
        _recurringDateUpdatedSubscription?.Dispose();
        _recurringDateDeletedSubscription?.Dispose();
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
        _singleDateAddedSubscription = eventBus.Listen<SingleDateAddedEvent>()
            .SelectMany(evt => Observable.FromAsync(ct => OnSingleDateAdded(evt, ct)))
            .Subscribe();
        _singleDateDeletedSubscription = eventBus.Listen<SingleDateDeletedEvent>()
            .SelectMany(evt => Observable.FromAsync(ct => OnSingleDateDeleted(evt, ct)))
            .Subscribe();
        _singleDateUpdatedSubscription = eventBus.Listen<SingleDateUpdatedEvent>()
            .SelectMany(evt => Observable.FromAsync(ct => OnSingleDateUpdated(evt, ct)))
            .Subscribe();
        _recurringDateAddedSubscription = eventBus.Listen<RecurringDateAddedEvent>()
            .SelectMany(evt => Observable.FromAsync(ct => OnRecurringDateAdded(evt, ct)))
            .Subscribe();
        _recurringDateDeletedSubscription = eventBus.Listen<RecurringDateDeletedEvent>()
            .SelectMany(evt => Observable.FromAsync(ct => OnRecurringDateDeleted(evt, ct)))
            .Subscribe();
        _recurringDateUpdatedSubscription = eventBus.Listen<RecurringDateUpdatedEvent>()
            .SelectMany(evt => Observable.FromAsync(ct => OnRecurringDateUpdated(evt, ct)))
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

    private async Task OnRecurringDateAdded(RecurringDateAddedEvent evt, CancellationToken ct)
    {
        await RefreshRecurringDatesAsync(evt.CalendarId, ct);
        CalculateReservedDays();
    }

    private async Task OnRecurringDateDeleted(RecurringDateDeletedEvent evt, CancellationToken ct)
    {
        await RefreshRecurringDatesAsync(evt.CalendarId, ct);
        CalculateReservedDays();
    }

    private async Task OnRecurringDateUpdated(RecurringDateUpdatedEvent evt, CancellationToken ct)
    {
        await RefreshRecurringDatesAsync(evt.CalendarId, ct);
        CalculateReservedDays();
    }

    private async Task OnSingleDateDeleted(SingleDateDeletedEvent evt, CancellationToken ct)
    {
        await RefreshSingleDatesAsync(evt.CalendarId, ct);
        CalculateReservedDays();
    }

    private async Task OnSingleDateUpdated(SingleDateUpdatedEvent evt, CancellationToken ct)
    {
        await RefreshSingleDatesAsync(evt.CalendarId, ct);
        CalculateReservedDays();
    }

    private async Task OnSingleDateAdded(SingleDateAddedEvent evt, CancellationToken ct)
    {
        await RefreshSingleDatesAsync(evt.CalendarId, ct);
        CalculateReservedDays();
    }

    private async Task RefreshRecurringDatesAsync(Guid calendarId, CancellationToken ct)
    {
        var response = await _mediator.Send(new GetRecurringDatesQuery(calendarId), ct);
        if (response.IsSuccess)
        {
            RecurringDates.Clear();
            SelectedCalendar?.RecurringDates.Clear();
            foreach (var recurringDateDto in response.RecurringDates)
            {
                var recurringDate = _calendarViewModelFactory.CreateRecurringDate(recurringDateDto);
                RecurringDates.Add(recurringDate);
                SelectedCalendar?.RecurringDates.Add(recurringDate);
            }
        }
    }

    private async Task RefreshSingleDatesAsync(Guid calendarId, CancellationToken ct)
    {
        var response = await _mediator.Send(new GetSingleDatesQuery(calendarId), ct);
        if (response.IsSuccess)
        {
            SingleDates.Clear();
            SelectedCalendar?.SingleDates.Clear();
            foreach (var singleDate in response.SingleDates)
            {
                var singleDateViewModel = _calendarViewModelFactory.CreateSingleDate(singleDate);
                SingleDates.Add(singleDateViewModel);
                SelectedCalendar?.SingleDates.Add(singleDateViewModel);
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

        SingleDates.Clear();
        RecurringDates.Clear();
        foreach (var calendar in SelectedCalendar.SingleDates)
        {
            SingleDates.Add(calendar);
        }

        foreach (var calendar in SelectedCalendar.RecurringDates)
        {
            RecurringDates.Add(calendar);
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
    private void OnAddCalendar()
    {
        _dialogNavigationService.NavigateTo(_calendarDialogViewModelsFactory.CreateAddCalendarViewModel());
    }

    [RelayCommand]
    private void OnUpdateCalendar(Guid id)
    {
        if (SelectedCalendar is null)
        {
            return;
        }

        var updateCalendarViewModel = _calendarDialogViewModelsFactory.CreateUpdateCalendarViewModel();
        updateCalendarViewModel.LoadData(SelectedCalendar);
        _dialogNavigationService.NavigateTo(updateCalendarViewModel);
    }

    [RelayCommand]
    private void OnDeleteCalendar(Guid id)
    {
        if (SelectedCalendar is null)
        {
            return;
        }

        var deleteCalendarViewModel = _calendarDialogViewModelsFactory.CreateDeleteCalendarViewModel();
        deleteCalendarViewModel.LoadData(SelectedCalendar);
        _dialogNavigationService.NavigateTo(deleteCalendarViewModel);
    }

    [RelayCommand]
    private void OnAddRecurringDate()
    {
        var calendarId = Calendars.FirstOrDefault(x => x.IsSelected)?.CalendarId;
        if (calendarId is null)
        {
            return;
        }

        var viewModel = _calendarDialogViewModelsFactory.CreateAddRecurringDateViewModel();
        viewModel.LoadData(calendarId.Value);
        _dialogNavigationService.NavigateTo(viewModel);
    }

    [RelayCommand]
    private void OnDeleteRecurringDate(Guid recurringDateId)
    {
        var recurringDateViewModel = SelectedCalendar?.RecurringDates.FirstOrDefault(x => x.RecurringDateId == recurringDateId);
        if (recurringDateViewModel is null)
        {
            return;
        }

        var deleteRecurringDateViewModel = _calendarDialogViewModelsFactory.CreateDeleteRecurringDateViewModel();
        deleteRecurringDateViewModel.LoadData(recurringDateViewModel);
        _dialogNavigationService.NavigateTo(deleteRecurringDateViewModel);
    }

    [RelayCommand]
    private void OnEditRecurringDate(Guid recurringDateId)
    {
        var recurringDateViewModel = SelectedCalendar?.RecurringDates.FirstOrDefault(x => x.RecurringDateId == recurringDateId);
        if (recurringDateViewModel is null)
        {
            return;
        }

        var updateRecurringDateViewModel = _calendarDialogViewModelsFactory.CreateUpdateRecurringDateViewModel();
        updateRecurringDateViewModel.LoadData(recurringDateViewModel);
        _dialogNavigationService.NavigateTo(updateRecurringDateViewModel);
    }

    [RelayCommand]
    private void OnAddSingleDate()
    {
        var calendarId = Calendars.FirstOrDefault(x => x.IsSelected)?.CalendarId;
        if (calendarId is null)
        {
            return;
        }

        var viewModel = _calendarDialogViewModelsFactory.CreateAddSingleDateViewModel();
        viewModel.LoadData(calendarId.Value);
        _dialogNavigationService.NavigateTo(viewModel);
    }

    [RelayCommand]
    private void OnDeleteSingleDate(Guid singleDateId)
    {
        var singleDateViewModel = SelectedCalendar?.SingleDates.FirstOrDefault(x => x.SingleDateId == singleDateId);
        if (singleDateViewModel is null)
        {
            return;
        }

        var deleteSingleDateViewModel = _calendarDialogViewModelsFactory.CreateDeleteSingleDateViewModel();
        deleteSingleDateViewModel.LoadData(singleDateViewModel);
        _dialogNavigationService.NavigateTo(deleteSingleDateViewModel);
    }

    [RelayCommand]
    private void OnEditSingleDate(Guid singleDateId)
    {
        var singleDateViewModel = SelectedCalendar?.SingleDates.FirstOrDefault(x => x.SingleDateId == singleDateId);
        if (singleDateViewModel is null)
        {
            return;
        }

        var updateSingleDateViewModel = _calendarDialogViewModelsFactory.CreateUpdateSingleDateViewModel();
        updateSingleDateViewModel.LoadData(singleDateViewModel);
        _dialogNavigationService.NavigateTo(updateSingleDateViewModel);
    }

    [RelayCommand]
    private void OnClose()
    {
        IsDialogOpen = false;
        _dialogNavigationService.CloseView();
    }
}
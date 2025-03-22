using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Linq;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddCalendarRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddRecurringDatesRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddSingleDateRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteCalendarRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.DeleteSingleDateRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateCalendarRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.UpdateSingleDateRequest;
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

public partial class ManageCalendarsViewModel : ObservableValidator, IPageViewModel, IDisposable
{
    private readonly IDisposable _calendarAddedSubscription;
    private readonly IDisposable _calendarDeletedSubscription;
    private readonly ICalendarDialogViewModelsFactory _calendarDialogViewModelsFactory;
    private readonly IDisposable _calendarUpdatedSubscription;
    private readonly ICalendarViewModelFactory _calendarViewModelFactory;
    private readonly INavigationService<IDialogViewModel> _dialogNavigationService;
    private readonly IMediator _mediator;
    private readonly IDisposable _recurringDateAddedSubscription;
    private readonly IDisposable _singleDateAddedSubscription;
    private readonly IDisposable _singleDateDeletedSubscription;
    private readonly IDisposable _singleDateUpdatedSubscription;

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
        ICalendarDialogViewModelsFactory calendarDialogViewModelsFactory)
    {
        _mediator = mediator;
        _dialogNavigationService = dialogNavigationService;
        _calendarViewModelFactory = calendarViewModelFactory;
        _calendarDialogViewModelsFactory = calendarDialogViewModelsFactory;
        Calendars.CollectionChanged += CalendarsOnCollectionChanged;

        _calendarAddedSubscription = eventBus.Listen<CalendarAddedEvent>()
            .SelectMany(_ => Observable.FromAsync(OnCalendarAdded))
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
    }

    public string CalendarName => SelectedCalendar?.Name ?? string.Empty;

    public bool IsCalendarSelected => SelectedCalendar is not null;

    public bool CalendarIsOnly => SelectedCalendar?.IsOnly ?? false;

    public FullyObservableCollection<CalendarViewModel> Calendars { get; } = [];

    public ObservableCollection<SingleDateViewModel> SingleDates { get; } = [];

    public ObservableCollection<RecurringDateViewModel> RecurringDates { get; } = [];

    public ObservableCollection<CategorizedDate> ReservedDates { get; set; } = [];

    public Guid SelectedCalendarId => SelectedCalendar?.CalendarId ?? Guid.Empty;

    public void Dispose()
    {
        _calendarAddedSubscription.Dispose();
        _calendarDeletedSubscription.Dispose();
        _calendarUpdatedSubscription.Dispose();
        _singleDateAddedSubscription.Dispose();
        _singleDateUpdatedSubscription.Dispose();
        _recurringDateAddedSubscription.Dispose();
    }

    public bool IsActive { get; set; }
    public string Icon => "CalendarClock";
    public string Name => "Calendars";

    public async Task LoadDataAsync(CancellationToken ct)
    {
        await LoadCalendars(ct);
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

    private async Task OnSingleDateAdded(SingleDateAddedEvent evt, CancellationToken ct)
    {
        await RefreshSingleDatesAsync(evt.CalendarId, ct);
        CalculateReservedDays();
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

    private async Task OnCalendarAdded(CancellationToken ct)
    {
        await LoadCalendars(ct);
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

        if (previouslySelectedCalendarId is not null)
        {
            var selectedCalendar = Calendars.FirstOrDefault(x => x.CalendarId == previouslySelectedCalendarId);
            if (selectedCalendar is not null)
            {
                selectedCalendar.IsSelected = true;
            }
        }
    }

    private void CalculateReservedDays()
    {
        ReservedDates.Clear();
        if (SelectedCalendar is null)
        {
            return;
        }

        foreach (var singleDate in SelectedCalendar.SingleDates)
        {
            if (singleDate.Date is null)
            {
                continue;
            }

            ReservedDates.Add(new CategorizedDate(singleDate.Date.Value.ToDateTime(TimeOnly.MinValue), CategorizedDateCategory.SingleDate, singleDate.Description));
        }

        foreach (var recurringDate in SelectedCalendar.RecurringDates)
        {
            if (recurringDate.StartDate is null)
            {
                continue;
            }

            const int dateRelatedDefaultValue = 1;
            var currentDate = recurringDate.StartDate;
            var numberOfRepetitions = recurringDate.NumberOfRepetitions ?? dateRelatedDefaultValue;
            for (var i = 0; i < numberOfRepetitions; i++)
            {
                var durationDate = currentDate;
                var duration = recurringDate?.Duration ?? dateRelatedDefaultValue;
                for (var j = 0; j < duration; j++)
                {
                    ReservedDates.Add(
                        new CategorizedDate(
                            durationDate.Value.ToDateTime(TimeOnly.MinValue),
                            CategorizedDateCategory.RecurringDate,
                            recurringDate?.Description ?? string.Empty));

                    durationDate = durationDate.Value.AddDays(1);
                }

                // Move to the next repetition period
                currentDate = currentDate.Value.AddDays(recurringDate?.RepetitionPeriod ?? dateRelatedDefaultValue);
            }
        }
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
    private void OnDeleteRecurringDate(Guid recurringDateId)
    {
    }

    [RelayCommand]
    private void OnEditRecurringDate(Guid recurringDateId)
    {
    }

    [RelayCommand]
    private void OnAddCalendar()
    {
        _dialogNavigationService.NavigateTo(_calendarDialogViewModelsFactory.CreateAddCalendarViewModel());
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
}
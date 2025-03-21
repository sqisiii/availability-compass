using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Linq;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddCalendarRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddRecurringDatesRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddSingleDateRequest;
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
    private readonly ICalendarDialogViewModelsFactory _calendarDialogViewModelsFactory;
    private readonly ICalendarViewModelFactory _calendarViewModelFactory;
    private readonly INavigationService<IDialogViewModel> _dialogNavigationService;
    private readonly IMediator _mediator;
    private readonly IDisposable _recurringDateAddedSubscription;
    private readonly IDisposable _singleDateAddedSubscription;

    [ObservableProperty]
    private string _calendarName = string.Empty;

    [ObservableProperty]
    private bool _isCalendarSelected;

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
        _singleDateAddedSubscription = eventBus.Listen<SingleDateAddedEvent>()
            .SelectMany(evt => Observable.FromAsync(ct => OnSingleDateAdded(evt, ct)))
            .Subscribe();
        _recurringDateAddedSubscription = eventBus.Listen<RecurringDateAddedEvent>()
            .SelectMany(evt => Observable.FromAsync(ct => OnRecurringDateAdded(evt, ct)))
            .Subscribe();
    }

    public FullyObservableCollection<CalendarViewModel> Calendars { get; } = [];

    public ObservableCollection<SingleDateViewModel> SingleDates { get; } = [];

    public ObservableCollection<RecurringDateViewModel> RecurringDates { get; } = [];

    public void Dispose()
    {
        _calendarAddedSubscription.Dispose();
        _singleDateAddedSubscription.Dispose();
        _recurringDateAddedSubscription.Dispose();
    }

    public bool IsActive { get; set; }
    public string Icon => "CalendarClock";
    public string Name => "Calendar";

    public async Task LoadDataAsync(CancellationToken ct)
    {
        await LoadCalendars(ct);
    }

    private async Task OnRecurringDateAdded(RecurringDateAddedEvent evt, CancellationToken ct)
    {
        await RefreshRecurringDatesAsync(evt.CalendarId, ct);
    }

    private async Task RefreshRecurringDatesAsync(Guid calendarId, CancellationToken ct)
    {
        var response = await _mediator.Send(new GetRecurringDatesQuery(calendarId), ct);
        if (response.IsSuccess)
        {
            RecurringDates.Clear();
            _selectedCalendar?.RecurringDates.Clear();
            foreach (var recurringDateDto in response.RecurringDates)
            {
                var recurringDate = _calendarViewModelFactory.CreateRecurringDate(recurringDateDto);
                RecurringDates.Add(recurringDate);
                _selectedCalendar?.RecurringDates.Add(recurringDate);
            }
        }
    }

    private async Task OnSingleDateAdded(SingleDateAddedEvent evt, CancellationToken ct)
    {
        await RefreshSingleDatesAsync(evt.CalendarId, ct);
    }

    private async Task RefreshSingleDatesAsync(Guid calendarId, CancellationToken ct)
    {
        var response = await _mediator.Send(new GetSingleDatesQuery(calendarId), ct);
        if (response.IsSuccess)
        {
            SingleDates.Clear();
            _selectedCalendar?.SingleDates.Clear();
            foreach (var singleDate in response.SingleDates)
            {
                var singleDateViewModel = _calendarViewModelFactory.CreateSingleDate(singleDate);
                SingleDates.Add(singleDateViewModel);
                _selectedCalendar?.SingleDates.Add(singleDateViewModel);
            }
        }
    }

    private async Task OnCalendarAdded(CancellationToken ct)
    {
        await LoadCalendars(ct);
    }


    private void CalendarsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        _selectedCalendar = Calendars.FirstOrDefault(x => x.IsSelected);
        if (_selectedCalendar is null)
        {
            IsCalendarSelected = false;
            return;
        }

        IsCalendarSelected = true;
        SingleDates.Clear();
        RecurringDates.Clear();
        foreach (var calendar in _selectedCalendar.SingleDates)
        {
            SingleDates.Add(calendar);
        }

        foreach (var calendar in _selectedCalendar.RecurringDates)
        {
            RecurringDates.Add(calendar);
        }
    }

    private async Task LoadCalendars(CancellationToken ct)
    {
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
    }

    [RelayCommand]
    private void OnEditCalendar(Guid id)
    {
    }


    [RelayCommand]
    private void OnDeleteSingleDate(Guid id)
    {
    }

    [RelayCommand]
    private void OnEditSingleDate(Guid id)
    {
    }

    [RelayCommand]
    private void OnDeleteRecurringDate(Guid id)
    {
    }

    [RelayCommand]
    private void OnEditRecurringDate(Guid id)
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
        var calendarId = Calendars.FirstOrDefault(x => x.IsSelected)?.Id;
        if (calendarId is null)
        {
            return;
        }

        var viewModel = _calendarDialogViewModelsFactory.CreateAddRecurringDateViewModel();
        viewModel.CalendarId = calendarId.Value;
        _dialogNavigationService.NavigateTo(viewModel);
    }

    [RelayCommand]
    private void OnAddSingleDate()
    {
        var calendarId = Calendars.FirstOrDefault(x => x.IsSelected)?.Id;
        if (calendarId is null)
        {
            return;
        }

        var viewModel = _calendarDialogViewModelsFactory.CreateAddSingleDateViewModel();
        viewModel.CalendarId = calendarId.Value;
        _dialogNavigationService.NavigateTo(viewModel);
    }
}
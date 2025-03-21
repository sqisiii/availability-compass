using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive.Linq;
using AvailabilityCompass.Core.Features.ManageCalendars.Commands.AddCalendarRequest;
using AvailabilityCompass.Core.Features.ManageCalendars.Queries.GetCalendarsQuery;
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

    [ObservableProperty]
    private string _calendarName = string.Empty;

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
    }

    public FullyObservableCollection<CalendarViewModel> Calendars { get; } = [];

    public ObservableCollection<SingleDateViewModel> SingleCalendars { get; } = [];

    public ObservableCollection<RecurringDateViewModel> RecurringCalendars { get; } = [];

    public void Dispose() => _calendarAddedSubscription.Dispose();

    public bool IsActive { get; set; }
    public string Icon => "CalendarClock";
    public string Name => "Calendar";

    public async Task LoadDataAsync(CancellationToken ct)
    {
        await LoadCalendars(ct);
    }

    private async Task OnCalendarAdded()
    {
        await LoadCalendars(ct);
    }

    private void CalendarsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        var selectedCalendar = Calendars.FirstOrDefault(x => x.IsSelected);
        if (selectedCalendar is null)
        {
            return;
        }

        SingleCalendars.Clear();
        RecurringCalendars.Clear();
        foreach (var calendar in selectedCalendar.SingleDates)
        {
            SingleCalendars.Add(calendar);
        }

        foreach (var calendar in selectedCalendar.RecurringDates)
        {
            RecurringCalendars.Add(calendar);
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
            Calendars.Add(_calendarViewModelFactory.Create(calendar));
        }
    }

    [RelayCommand]
    private void OnDeleteSingleDate(string id)
    {
    }

    [RelayCommand]
    private void OnEditSingleDate(string id)
    {
    }

    [RelayCommand]
    private void OnDeleteRecurringDate(string id)
    {
    }

    [RelayCommand]
    private void OnEditRecurringDate(string id)
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
        _dialogNavigationService.NavigateTo(_calendarDialogViewModelsFactory.CreateAddRecurringDateViewModel());
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
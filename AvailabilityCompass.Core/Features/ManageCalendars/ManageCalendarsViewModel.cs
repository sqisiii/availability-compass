using System.Collections.ObjectModel;
using AvailabilityCompass.Core.Shared;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MediatR;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

public partial class ManageCalendarsViewModel : ObservableValidator, IPageViewModel
{
    private readonly IMediator _mediator;

    public ManageCalendarsViewModel(IMediator mediator)
    {
        _mediator = mediator;
        Calendars.Add(new CalendarViewModel() { Name = "John's" });
        Calendars.Add(new CalendarViewModel() { Name = "Marry's" });
        Calendars.Add(new CalendarViewModel() { Name = "Common holidays" });
        Calendars.Add(new CalendarViewModel() { Name = "Christmas" });
        Calendars.Add(new CalendarViewModel() { Name = "Vacation days" });
        Calendars.Add(new CalendarViewModel() { Name = "Andy's" });
        Calendars.Add(new CalendarViewModel() { Name = "Kris's" });
        Calendars.Add(new CalendarViewModel() { Name = "Juliet's" });
    }

    public FullyObservableCollection<CalendarViewModel> Calendars { get; } = [];

    public ObservableCollection<SingleDateViewModel> SingleCalendars { get; } = [];

    public ObservableCollection<RecurringDateViewModel> RecurringCalendars { get; } = [];

    public bool IsActive { get; set; }
    public string Icon => "CalendarClock";
    public string Name => "Calendar";

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
        Calendars.Add(new CalendarViewModel());
    }

    [RelayCommand]
    private void OnAddRecurringDate()
    {
    }

    [RelayCommand]
    private void OnAddSingleDate()
    {
    }
}
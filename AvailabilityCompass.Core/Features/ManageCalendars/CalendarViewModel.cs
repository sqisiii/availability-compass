using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

public partial class CalendarViewModel : ObservableObject
{
    [ObservableProperty]
    private Guid _id;

    [ObservableProperty]
    private bool _isOnly;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private string _name = string.Empty;

    public string Type => IsOnly ? "Only" : "Except";


    public List<SingleDateViewModel> SingleDates { get; set; } = [];
    public List<RecurringDateViewModel> RecurringDates { get; set; } = [];
}
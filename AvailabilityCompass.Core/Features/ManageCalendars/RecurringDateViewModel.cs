using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

public partial class RecurringDateViewModel : ObservableObject
{
    [ObservableProperty]
    private int _daysCount;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _id = string.Empty;

    [ObservableProperty]
    private DateTime _startDate;
}
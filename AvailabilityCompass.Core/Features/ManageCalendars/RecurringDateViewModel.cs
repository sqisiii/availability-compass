using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

public partial class RecurringDateViewModel : ObservableObject
{
    [ObservableProperty]
    private int _daysCount;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private Guid _id;

    [ObservableProperty]
    private DateOnly _startDate;
}
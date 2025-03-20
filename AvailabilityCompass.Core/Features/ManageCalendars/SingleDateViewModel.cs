using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

public partial class SingleDateViewModel : ObservableObject
{
    [ObservableProperty]
    private string _id  = string.Empty;
    [ObservableProperty]
    private string _description  = string.Empty;
    [ObservableProperty]
    private string _date  = string.Empty;
}
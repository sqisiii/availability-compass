using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

public partial class SingleDateViewModel : ObservableObject
{
    [ObservableProperty]
    private DateOnly _date;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private Guid _id;
}
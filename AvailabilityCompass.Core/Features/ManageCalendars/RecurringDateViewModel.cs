using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

public partial class RecurringDateViewModel : ObservableObject
{
    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private int _duration;

    [ObservableProperty]
    private Guid _id;

    [ObservableProperty]
    private int _numberOfRepetitions;

    [ObservableProperty]
    private int _repetitionPeriod;

    [ObservableProperty]
    private DateOnly _startDate;
}
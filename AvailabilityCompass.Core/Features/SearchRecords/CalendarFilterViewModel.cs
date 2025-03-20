using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.SearchRecords;

public partial class CalendarFilterViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _type = string.Empty;

    public CalendarFilterViewModel(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}
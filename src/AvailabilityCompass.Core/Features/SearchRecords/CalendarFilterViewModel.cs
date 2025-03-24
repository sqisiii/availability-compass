using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.SearchRecords;

public partial class CalendarFilterViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isOnly;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private string _name = string.Empty;

    public CalendarFilterViewModel(Guid id)
    {
        Id = id;
    }

    public string Type => IsOnly ? "Only" : "Exclude";

    public Guid Id { get; }
}
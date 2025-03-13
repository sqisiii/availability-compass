using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.Search;

public partial class CalendarViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _type = string.Empty;

    public CalendarViewModel(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}
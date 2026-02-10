using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.SearchRecords;

/// <summary>
/// ViewModel representing a calendar filter option in the search interface.
/// </summary>
public partial class CalendarFilterViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsMarkMode))]
    [NotifyPropertyChangedFor(nameof(IsFilterMode))]
    private bool _isMarkOnly;

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

    public string Type => IsOnly ? "Available days" : "Blocked days";

    public bool IsMarkMode => IsMarkOnly;

    public bool IsFilterMode => !IsMarkOnly;

    public Guid Id { get; }

    partial void OnIsMarkOnlyChanged(bool value)
    {
        if (value && !IsSelected)
        {
            IsSelected = true;
        }
    }

    partial void OnIsSelectedChanged(bool value)
    {
        if (!value && IsMarkOnly)
        {
            IsMarkOnly = false;
        }
    }
}
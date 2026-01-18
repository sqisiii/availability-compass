using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.SearchRecords.FilterFormElements;

/// <summary>
/// Represents a selectable option within a multi-select form element.
/// </summary>
public partial class FormElementSelectOption : ObservableObject
{
    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    private string _name = string.Empty;

    public FormElementSelectOption(string name, bool isSelected)
    {
        Name = name;
        IsSelected = isSelected;
    }
}
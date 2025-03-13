using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.Search.FilterFormElements;

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
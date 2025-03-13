using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.Search.FilterFormElements;

public partial class FormElement : ObservableObject
{
    [ObservableProperty]
    private string? _textValue = string.Empty;

    public string Label { get; set; } = string.Empty;
    public FormElementType Type { get; set; }
    public ObservableCollection<FormElementSelectOption> Options { get; set; } = [];

    public ObservableCollection<string> SelectedOptions { get; } = [];
}
using System.Collections.ObjectModel;
using AvailabilityCompass.Core.Shared;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.SearchRecords.FilterFormElements;

/// <summary>
/// Represents a form element used for filtering search results.
/// </summary>
public partial class FormElement : ObservableObject
{
    [ObservableProperty]
    private string? _textValue = string.Empty;

    public string Label { get; set; } = string.Empty;
    public FormElementType Type { get; set; }
    public FullyObservableCollection<FormElementSelectOption> Options { get; set; } = [];

    public ObservableCollection<string> SelectedOptions { get; } = [];
}
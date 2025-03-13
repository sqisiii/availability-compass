using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.Search.FilterFormElements;

public class FormGroup : ObservableObject
{
    public string Title { get; set; } = string.Empty;
    public ObservableCollection<FormElement> Elements { get; set; } = new();
}
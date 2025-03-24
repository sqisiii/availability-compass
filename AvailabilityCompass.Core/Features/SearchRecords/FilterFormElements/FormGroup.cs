using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.SearchRecords.FilterFormElements;

public class FormGroup : ObservableObject
{
    public string Title { get; init; } = string.Empty;

    public string SourceId { get; init; } = string.Empty;
    public ObservableCollection<FormElement> Elements { get; set; } = [];
}
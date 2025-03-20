using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

public class CalendarViewModel : ObservableObject
{
    public bool IsChecked { get; set; }
    public string Name { get; set; } = string.Empty;
}
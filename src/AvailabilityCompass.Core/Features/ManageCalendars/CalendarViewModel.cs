using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

/// <summary>
/// View model representing a calendar with its selection state and date entries.
/// </summary>
public partial class CalendarViewModel : ObservableValidator
{
    [ObservableProperty]
    private bool _isOnly;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Name is required")]
    private string _name = string.Empty;

    public string Type => IsOnly ? "Available days" : "Blocked days";

    public Guid CalendarId { get; init; }


    public List<DateEntryViewModel> DateEntries { get; init; } = [];
}
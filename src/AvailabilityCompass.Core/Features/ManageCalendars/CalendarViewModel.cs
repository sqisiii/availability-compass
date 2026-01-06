using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

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


    public List<SingleDateViewModel> SingleDates { get; init; } = [];
    public List<RecurringDateViewModel> RecurringDates { get; init; } = [];
}
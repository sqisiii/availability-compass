using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

public partial class CalendarViewModel : ObservableValidator
{
    [ObservableProperty]
    private Guid _id;

    [ObservableProperty]
    private bool _isOnly;

    [ObservableProperty]
    private bool _isSelected;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Name is required")]
    private string _name = string.Empty;

    public string Type => IsOnly ? "Only" : "Except";


    public List<SingleDateViewModel> SingleDates { get; set; } = [];
    public List<RecurringDateViewModel> RecurringDates { get; set; } = [];
}
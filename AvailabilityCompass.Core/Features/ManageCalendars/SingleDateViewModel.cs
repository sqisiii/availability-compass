using System.ComponentModel.DataAnnotations;
using AvailabilityCompass.Core.Shared.ValidationAttributes;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

public partial class SingleDateViewModel : ObservableValidator
{
    [ObservableProperty]
    [DateValidation]
    [Required(ErrorMessage = "Date is required")]
    [NotifyDataErrorInfo]
    private string? _dateString;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Description is required")]
    private string _description = string.Empty;

    [ObservableProperty]
    private Guid _singleDateId;

    public DateOnly? Date
    {
        get => DateString != null ? DateOnly.TryParse(DateString, out var date) ? date : null : null;
        set => DateString = value?.ToString();
    }

    public Guid CalendarId { get; set; }
}
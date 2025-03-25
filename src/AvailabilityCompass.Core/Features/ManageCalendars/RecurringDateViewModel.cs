using System.ComponentModel.DataAnnotations;
using AvailabilityCompass.Core.Shared.ValidationAttributes;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

public partial class RecurringDateViewModel : ObservableValidator
{
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Description is required")]
    private string _description = string.Empty;


    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Duration is required")]
    [Range(1, 365)]
    private int? _duration;


    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Range(0, 365)]
    private int? _frequency;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Range(0, int.MaxValue)]
    private int? _numberOfRepetitions;

    [ObservableProperty]
    private Guid _recurringDateId;

    [ObservableProperty]
    [DateValidation]
    [Required(ErrorMessage = "Start date is required")]
    [NotifyDataErrorInfo]
    private string? _startDateString;


    public DateOnly? StartDate
    {
        get => StartDateString != null ? DateOnly.TryParse(StartDateString, out var date) ? date : null : null;
        set => StartDateString = value?.ToString();
    }

    public Guid CalendarId { get; set; }
}
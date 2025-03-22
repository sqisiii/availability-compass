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
    private Guid _id;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Number of repetitions is required")]
    [Range(1, int.MaxValue)]
    private int? _numberOfRepetitions;


    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Repetition period is required")]
    [Range(1, 365)]
    private int? _repetitionPeriod;

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
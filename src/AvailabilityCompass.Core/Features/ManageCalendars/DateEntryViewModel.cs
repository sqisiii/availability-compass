using System.ComponentModel.DataAnnotations;
using AvailabilityCompass.Core.Shared.ValidationAttributes;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvailabilityCompass.Core.Features.ManageCalendars;

public partial class DateEntryViewModel : ObservableValidator
{
    [ObservableProperty]
    private Guid _calendarId;

    [ObservableProperty]
    private Guid _dateEntryId;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Required(ErrorMessage = "Description is required")]
    private string _description = string.Empty;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days")]
    private int _duration = 1;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Range(1, 365, ErrorMessage = "Frequency must be between 1 and 365 days")]
    private int? _frequency;

    [ObservableProperty]
    private bool _isRecurring;

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [Range(0, int.MaxValue, ErrorMessage = "Number of repetitions must be 0 or greater")]
    private int _numberOfRepetitions;

    [ObservableProperty]
    [DateValidation]
    [Required(ErrorMessage = "Start date is required")]
    [NotifyDataErrorInfo]
    private string? _startDateString;

    public DateOnly StartDate
    {
        get => StartDateString != null && DateOnly.TryParse(StartDateString, out var date) ? date : throw new InvalidOperationException();
        init => StartDateString = value.ToString("yyyy-MM-dd");
    }

    private DateOnly? EndDate => StartDate.AddDays(Duration - 1);

    public string DateRangeDisplay => Duration > 1
        ? $"{StartDate:yyyy-MM-dd} - {EndDate:yyyy-MM-dd}"
        : StartDateString ?? "";

    private DateOnly? NextOccurrence
    {
        get
        {
            if (!IsRecurring || Frequency is null) return null;

            var today = DateOnly.FromDateTime(DateTime.Today);
            var current = StartDate;

            // Find a next occurrence after today
            for (var i = 0; i <= NumberOfRepetitions; i++)
            {
                var periodEnd = current.AddDays(Duration - 1);
                if (periodEnd >= today) return current;
                current = current.AddDays(Frequency.Value);
            }

            return null; // No future occurrences
        }
    }

    public string NextOccurrenceDisplay => NextOccurrence is { } next
        ? $"Next: {next:yyyy-MM-dd}"
        : "";
}
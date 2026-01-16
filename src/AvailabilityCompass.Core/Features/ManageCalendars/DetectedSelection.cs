namespace AvailabilityCompass.Core.Features.ManageCalendars;

/// <summary>
/// Represents a detected date selection, which may be a single date or a consecutive period.
/// </summary>
/// <param name="StartDate">The start date of the selection.</param>
/// <param name="Duration">The number of consecutive days in the selection (1 for a single date).</param>
public record DetectedSelection(DateOnly StartDate, int Duration)
{
    private DateOnly EndDate => StartDate.AddDays(Duration - 1);

    /// <summary>
    /// Indicates whether this selection spans multiple days.
    /// </summary>
    public bool IsPeriod => Duration > 1;

    /// <summary>
    /// Gets a human-readable display text for this selection.
    /// </summary>
    public string DisplayText => IsPeriod
        ? $"{StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd} ({Duration} days)"
        : $"{StartDate:yyyy-MM-dd}";
}
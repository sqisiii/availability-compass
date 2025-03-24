namespace AvailabilityCompass.Core.Features.ManageCalendars;

/// <summary>
/// Represents a date with an associated category and explanatory tooltip.
/// </summary>
/// <remarks>
/// This class is used to categorize dates for calendar management features,
/// allowing dates to be classified as single, recurring, or inverted.
/// </remarks>
public class CategorizedDate
{
    public CategorizedDate(DateTime date, CategorizedDateCategory category, string tooltip)
    {
        Date = date;
        Category = category;
        Tooltip = tooltip;
    }

    public DateTime Date { get; }
    public CategorizedDateCategory Category { get; }
    public string Tooltip { get; }
}

public enum CategorizedDateCategory
{
    SingleDate,
    RecurringDate,
    Inverted
}
namespace AvailabilityCompass.Core.Features.ManageCalendars;

public class CategorizedDate
{
    public CategorizedDate(DateTime date, CategorizedDateCategory category, string tooltip)
    {
        Date = date;
        Category = category;
        Tooltip = tooltip;
    }

    public DateTime Date { get; set; }
    public CategorizedDateCategory Category { get; set; }
    public string Tooltip { get; set; }
}

public enum CategorizedDateCategory
{
    SingleDate,
    RecurringDate,
    Inverted
}
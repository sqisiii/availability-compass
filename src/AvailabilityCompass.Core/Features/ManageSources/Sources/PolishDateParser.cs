namespace AvailabilityCompass.Core.Features.ManageSources.Sources;

/// <summary>
/// Utility for parsing Polish date strings in various formats.
/// Supports both nominative and genitive forms of Polish month names.
/// </summary>
public static class PolishDateParser
{
    private static readonly Dictionary<string, int> PolishMonthNames = new(StringComparer.OrdinalIgnoreCase)
    {
        // Genitive forms (used in dates like "15 stycznia")
        { "stycznia", 1 },
        { "lutego", 2 },
        { "marca", 3 },
        { "kwietnia", 4 },
        { "maja", 5 },
        { "czerwca", 6 },
        { "lipca", 7 },
        { "sierpnia", 8 },
        { "września", 9 },
        { "października", 10 },
        { "listopada", 11 },
        { "grudnia", 12 },
        // Nominative forms
        { "styczeń", 1 },
        { "luty", 2 },
        { "marzec", 3 },
        { "kwiecień", 4 },
        { "maj", 5 },
        { "czerwiec", 6 },
        { "lipiec", 7 },
        { "sierpień", 8 },
        { "wrzesień", 9 },
        { "październik", 10 },
        { "listopad", 11 },
        { "grudzień", 12 }
    };

    /// <summary>
    /// Gets the month number (1-12) for a Polish month name.
    /// </summary>
    /// <param name="monthName">The Polish month name (case-insensitive).</param>
    /// <returns>The month number (1-12), or the current month if not recognized.</returns>
    public static int GetMonthNumber(string monthName)
    {
        return PolishMonthNames.TryGetValue(monthName, out var month)
            ? month
            : DateTime.Now.Month;
    }

    /// <summary>
    /// Tries to parse a Polish date string in the format "day month [year]".
    /// </summary>
    /// <param name="dateText">The date text to parse (e.g., "15 maja 2025" or "15 maja").</param>
    /// <param name="result">The parsed date, or <see cref="DateOnly.MinValue"/> if parsing fails.</param>
    /// <returns>True if parsing succeeded; otherwise, false.</returns>
    public static bool TryParsePolishDate(string? dateText, out DateOnly result)
    {
        result = DateOnly.MinValue;

        if (string.IsNullOrEmpty(dateText))
            return false;

        var parts = dateText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2 || !int.TryParse(parts[0], out var day))
            return false;

        var month = GetMonthNumber(parts[1]);
        var year = parts.Length > 2 && int.TryParse(parts[2], out var y) ? y : DateTime.Now.Year;

        if (day <= 0 || day > 31 || month <= 0 || month > 12)
            return false;

        try
        {
            result = new DateOnly(year, month, day);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

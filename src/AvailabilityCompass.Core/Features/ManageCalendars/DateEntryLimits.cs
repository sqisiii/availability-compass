namespace AvailabilityCompass.Core.Features.ManageCalendars;

/// <summary>
/// Bounds for recurring date entries. Without them a single entry can expand to
/// millions of dates, freezing the UI thread (or OOM-crashing) on every calendar
/// selection, date click, and search.
/// </summary>
public static class DateEntryLimits
{
    /// <summary>Upper bound enforced by validation on user-entered repetitions.</summary>
    public const int MaxRepetitions = 1000;

    /// <summary>
    /// Defensive cap applied inside expansion loops so pre-existing (or imported)
    /// rows that exceed the validation bound still cannot freeze the app.
    /// </summary>
    public const int MaxExpandedDatesPerEntry = 20_000;
}
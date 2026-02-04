namespace AvailabilityCompass.Core.Features.Tutorial;

/// <summary>
/// Triggers that can cause tutorial steps to auto-advance.
/// </summary>
public enum AppTutorialTrigger
{
    /// <summary>
    /// The current dialog changed (opened/closed/switched).
    /// </summary>
    DialogChanged,

    /// <summary>
    /// A source was refreshed.
    /// </summary>
    SourceRefreshed,

    /// <summary>
    /// A calendar was added.
    /// </summary>
    CalendarAdded,

    /// <summary>
    /// The add calendar form was expanded.
    /// </summary>
    CalendarFormExpanded,

    /// <summary>
    /// A calendar was selected.
    /// </summary>
    CalendarSelected,

    /// <summary>
    /// A date entry was added.
    /// </summary>
    DateEntryAdded,

    /// <summary>
    /// A filter was selected.
    /// </summary>
    FilterSelected,

    /// <summary>
    /// A search was performed.
    /// </summary>
    SearchPerformed
}
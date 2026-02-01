namespace AvailabilityCompass.Core.Features.Tutorial;

/// <summary>
/// Represents the currently open dialog in the application.
/// </summary>
public enum DialogType
{
    None,
    Sources,
    Calendars
}

/// <summary>
/// Captures the current application state used for the conditional tutorial flow.
/// </summary>
public record TutorialContext
{
    /// <summary>
    /// Whether any sources exist in the system.
    /// </summary>
    public bool HasSources { get; init; }

    /// <summary>
    /// Whether at least one source has trip data loaded.
    /// </summary>
    public bool HasSourcesWithData { get; init; }

    /// <summary>
    /// Whether the user has refreshed at least one source during this tutorial session.
    /// </summary>
    public bool HasRefreshedSource { get; init; }

    /// <summary>
    /// Whether any calendars exist.
    /// </summary>
    public bool HasCalendars { get; init; }

    /// <summary>
    /// Whether the current calendar has date entries.
    /// </summary>
    public bool HasCalendarEntries { get; init; }

    /// <summary>
    /// Currently open dialog.
    /// </summary>
    public DialogType CurrentDialog { get; init; } = DialogType.None;

    /// <summary>
    /// Whether the add calendar inline form is expanded.
    /// </summary>
    public bool IsAddCalendarExpanded { get; init; }

    /// <summary>
    /// Whether a calendar is currently selected.
    /// </summary>
    public bool IsCalendarSelected { get; init; }

    /// <summary>
    /// Whether the date entry editor modal is open.
    /// </summary>
    public bool IsEditorOpen { get; init; }

    /// <summary>
    /// Whether the search view has results displayed.
    /// </summary>
    public bool HasSearchResults { get; init; }

    /// <summary>
    /// Whether any source filter is currently selected.
    /// </summary>
    public bool HasSourceFilterSelected { get; init; }

    /// <summary>
    /// Whether any calendar filter is currently selected.
    /// </summary>
    public bool HasCalendarFilterSelected { get; init; }

    /// <summary>
    /// Creates a default context with all values set to their defaults.
    /// </summary>
    public static TutorialContext Default => new();
}
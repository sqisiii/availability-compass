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
    SearchPerformed,

    /// <summary>
    /// Dates were selected on the calendar widget.
    /// </summary>
    DatesSelected,

    /// <summary>
    /// The edit calendar form was expanded.
    /// </summary>
    CalendarEditStarted,

    /// <summary>
    /// The edit calendar form was closed (saved or canceled).
    /// </summary>
    CalendarEditCompleted,

    /// <summary>
    /// The delete confirmation dialog was opened.
    /// </summary>
    DeleteConfirmationOpened,

    /// <summary>
    /// The delete confirmation dialog was closed (confirmed or canceled).
    /// </summary>
    DeleteConfirmationClosed,

    /// <summary>
    /// The date entry editor was opened.
    /// </summary>
    EditorOpened,

    /// <summary>
    /// The date entry editor was closed (saved, canceled, or deleted).
    /// </summary>
    EditorClosed,

    /// <summary>
    /// The calendar filter section was expanded in the search view.
    /// </summary>
    CalendarFilterExpanded,

    /// <summary>
    /// The sources filter section was expanded in the search view.
    /// </summary>
    SourcesFilterExpanded,

    /// <summary>
    /// The global filters section was expanded in the search view.
    /// </summary>
    FiltersExpanded
}
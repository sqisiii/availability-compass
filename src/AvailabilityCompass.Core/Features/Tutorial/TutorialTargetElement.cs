namespace AvailabilityCompass.Core.Features.Tutorial;

/// <summary>
/// Identifies UI elements that can be highlighted during the tutorial.
/// </summary>
public enum TutorialTargetElement
{
    None,

    // Header
    SourcesHeaderButton,
    CalendarsHeaderButton,

    // Sources Dialog
    RefreshAllButton,
    SourceCardRefreshButton,

    // Calendars Dialog
    CalendarSelector,
    AddCalendarButton,
    AddCalendarForm,
    CalendarEditButton,
    CalendarDeleteButton,
    CalendarWidget,
    AddDaysButton,
    DateEntriesPanel,

    // Search View
    CalendarFilterSection,
    SourcesFilterSection,
    SourceFilterOptions,
    FiltersSection,
    ResultsSection,
    SearchButton
}

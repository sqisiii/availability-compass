namespace AvailabilityCompass.Core.Features.Tutorial;

/// <summary>
/// Identifies each step in the tutorial flow.
/// </summary>
public enum TutorialStepId
{
    // Phase 1: Sources
    Welcome,
    SourcesDialogRefreshButtons, // 1a: Point to Refresh All + first card's Refresh button
    PointToSourcesButton, // 1b: If already has sources, point to a header

    // Phase 2: Transition
    PointToCalendarsButton, // 2a: After refresh, point to Calendars
    WaitForSourceRefresh, // 2b: Remind to refresh sources

    // Phase 3: Calendars Dialog
    CalendarsOverview, // 3: Explain calendar types (allowed/blocked)
    ClickExistingCalendar, // 3a: Has calendar - click to edit
    ClickAddCalendarButton, // 3b: No calendar - click Add

    // Phase 4: Add Calendar Form
    AddCalendarFormExplanation, // 4: Name + checkbox + Create

    // Phase 5: Calendar Days View
    CalendarViewOverview, // 5: Edit/Delete + Calendar widget
    AddDaysExplanation, // 6a: Description + recurring
    DateEntriesExplanation, // 6b: Existing entries panel

    // Phase 6: Search View
    PointToSearchView, // Transition back
    CalendarFilterExplanation, // 7: Calendar selection
    SourcesFilterExplanation, // 8: Sources + selection boxes
    SourceFilterOptionsExplanation, // 8b: Source-specific filter options
    FiltersExplanation, // 9: Global filters
    SearchButtonExplanation, // 9b: Search button
    ResultsExplanation, // 10: Results cards

    TutorialComplete
}
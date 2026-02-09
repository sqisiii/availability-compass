namespace AvailabilityCompass.Core.Features.Tutorial;

/// <summary>
/// Static string constants for all tutorial step IDs.
/// Provides compile-time validation and IntelliSense support.
/// </summary>
public static class TutorialStepIds
{
    // Introduction group
    public const string Welcome = nameof(Welcome);
    public const string TutorialUsage = nameof(TutorialUsage);

    // Sources group
    public const string PointToSourcesButton = nameof(PointToSourcesButton);
    public const string SourcesDialogRefreshButtons = nameof(SourcesDialogRefreshButtons);
    public const string WaitForSourceRefresh = nameof(WaitForSourceRefresh);
    public const string PointToCalendarsButton = nameof(PointToCalendarsButton);

    // Calendars group
    public const string CalendarsOverview = nameof(CalendarsOverview);
    public const string ClickExistingCalendar = nameof(ClickExistingCalendar);
    public const string ClickAddCalendarButton = nameof(ClickAddCalendarButton);
    public const string AddCalendarFormExplanation = nameof(AddCalendarFormExplanation);
    public const string CalendarViewOverview = nameof(CalendarViewOverview);
    public const string EditCalendarStep = nameof(EditCalendarStep);
    public const string EditCalendarFormStep = nameof(EditCalendarFormStep);
    public const string DeleteCalendarStep = nameof(DeleteCalendarStep);
    public const string DeleteConfirmationStep = nameof(DeleteConfirmationStep);
    public const string SelectDatesExplanation = nameof(SelectDatesExplanation);
    public const string AddDaysExplanation = nameof(AddDaysExplanation);
    public const string AddDatesDialogExplanation = nameof(AddDatesDialogExplanation);
    public const string DateEntriesExplanation = nameof(DateEntriesExplanation);
    public const string ClickDateEntryStep = nameof(ClickDateEntryStep);
    public const string EditDateEntryExplanation = nameof(EditDateEntryExplanation);
    public const string PointToSearchView = nameof(PointToSearchView);

    // Search group
    public const string CalendarFilterExplanation = nameof(CalendarFilterExplanation);
    public const string CalendarTypesExplanation = nameof(CalendarTypesExplanation);
    public const string SourcesFilterExplanation = nameof(SourcesFilterExplanation);
    public const string SelectSourceStep = nameof(SelectSourceStep);
    public const string SourceFilterOptionsExplanation = nameof(SourceFilterOptionsExplanation);
    public const string FiltersExplanation = nameof(FiltersExplanation);
    public const string FiltersOptionsExplanation = nameof(FiltersOptionsExplanation);
    public const string SearchButtonExplanation = nameof(SearchButtonExplanation);
    public const string ResultsExplanation = nameof(ResultsExplanation);
    public const string NoResultsExplanation = nameof(NoResultsExplanation);

    // Complete group
    public const string TutorialComplete = nameof(TutorialComplete);
}
namespace AvailabilityCompass.Core.Features.Tutorial;

/// <summary>
/// Static string constants for all tutorial step IDs.
/// Provides compile-time validation and IntelliSense support.
/// </summary>
public static class TutorialStepIds
{
    // Introduction group
    public const string Welcome = nameof(Welcome);

    // Sources group
    public const string PointToSourcesButton = nameof(PointToSourcesButton);
    public const string SourcesDialogRefreshButtons = nameof(SourcesDialogRefreshButtons);
    public const string SourcesRefreshOptionalStep = nameof(SourcesRefreshOptionalStep);
    public const string WaitForSourceRefresh = nameof(WaitForSourceRefresh);

    // Calendars group
    public const string PointToCalendarsButton = nameof(PointToCalendarsButton);
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

    // Search group
    public const string PointToSearchView = nameof(PointToSearchView);
    public const string CalendarFilterExplanation = nameof(CalendarFilterExplanation);
    public const string CalendarTypesExplanation = nameof(CalendarTypesExplanation);
    public const string SourcesFilterExplanation = nameof(SourcesFilterExplanation);
    public const string SelectSourceStep = nameof(SelectSourceStep);
    public const string SourceFilterOptionsExplanation = nameof(SourceFilterOptionsExplanation);
    public const string FiltersExplanation = nameof(FiltersExplanation);
    public const string FiltersOptionsExplanation = nameof(FiltersOptionsExplanation);
    public const string SearchButtonExplanation = nameof(SearchButtonExplanation);
    public const string ResultsExplanation = nameof(ResultsExplanation);

    // Complete group
    public const string TutorialComplete = nameof(TutorialComplete);
}
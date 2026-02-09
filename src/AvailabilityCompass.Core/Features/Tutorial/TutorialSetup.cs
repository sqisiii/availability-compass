using Guidely.Core.Configuration;

namespace AvailabilityCompass.Core.Features.Tutorial;

/// <summary>
/// Configuration for tutorial step transitions.
/// </summary>
public static class TutorialSetup
{
    /// <summary>
    /// Configures the transitions between tutorial steps.
    /// </summary>
    public static void ConfigureTransitions(TransitionBuilder<AvailabilityCompassContext> builder)
    {
        // Introduction flow
        builder.From(TutorialStepIds.Welcome)
            .GoTo(TutorialStepIds.TutorialUsage);

        builder.From(TutorialStepIds.TutorialUsage)
            .GoToIf(TutorialStepIds.SourcesDialogRefreshButtons, ctx => ctx.CurrentDialog == DialogType.Sources)
            .GoTo(TutorialStepIds.PointToSourcesButton);

        // Sources flow
        builder.From(TutorialStepIds.PointToSourcesButton)
            .GoTo(TutorialStepIds.SourcesDialogRefreshButtons);

        builder.From(TutorialStepIds.SourcesDialogRefreshButtons)
            .GoToIf(TutorialStepIds.PointToCalendarsButton, ctx => ctx.HasRefreshedSource)
            .SkipTo(TutorialStepIds.PointToCalendarsButton)
            .GoTo(TutorialStepIds.WaitForSourceRefresh);

        builder.From(TutorialStepIds.WaitForSourceRefresh)
            .SkipTo(TutorialStepIds.PointToCalendarsButton)
            .GoTo(TutorialStepIds.PointToCalendarsButton);

        // Sources -> Calendars
        builder.From(TutorialStepIds.PointToCalendarsButton)
            .GoTo(TutorialStepIds.CalendarsOverview);

        // Calendars flow
        builder.From(TutorialStepIds.CalendarsOverview)
            .GoToIf(TutorialStepIds.ClickExistingCalendar, ctx => ctx.HasCalendars)
            .GoTo(TutorialStepIds.ClickAddCalendarButton);

        builder.From(TutorialStepIds.ClickExistingCalendar)
            .SkipTo(TutorialStepIds.CalendarViewOverview)
            .GoTo(TutorialStepIds.CalendarViewOverview);

        builder.From(TutorialStepIds.ClickAddCalendarButton)
            .GoTo(TutorialStepIds.AddCalendarFormExplanation);

        builder.From(TutorialStepIds.AddCalendarFormExplanation)
            .GoTo(TutorialStepIds.CalendarViewOverview);

        builder.From(TutorialStepIds.CalendarViewOverview)
            .GoTo(TutorialStepIds.EditCalendarStep);

        builder.From(TutorialStepIds.EditCalendarStep)
            .DisableBack()
            .SkipTo(TutorialStepIds.DeleteCalendarStep)
            .GoTo(TutorialStepIds.EditCalendarFormStep);

        builder.From(TutorialStepIds.EditCalendarFormStep)
            .GoTo(TutorialStepIds.DeleteCalendarStep);

        builder.From(TutorialStepIds.DeleteCalendarStep)
            .DisableBack()
            .SkipTo(TutorialStepIds.SelectDatesExplanation)
            .GoTo(TutorialStepIds.DeleteConfirmationStep);

        builder.From(TutorialStepIds.DeleteConfirmationStep)
            .DisableBack()
            .GoToIf(TutorialStepIds.CalendarsOverview, ctx => !ctx.HasCalendars)
            .GoTo(TutorialStepIds.SelectDatesExplanation);

        builder.From(TutorialStepIds.SelectDatesExplanation)
            .BackTo(TutorialStepIds.CalendarViewOverview)
            .SkipTo(TutorialStepIds.DateEntriesExplanation)
            .GoTo(TutorialStepIds.AddDaysExplanation);

        builder.From(TutorialStepIds.AddDaysExplanation)
            .SkipTo(TutorialStepIds.DateEntriesExplanation)
            .GoTo(TutorialStepIds.AddDatesDialogExplanation);

        builder.From(TutorialStepIds.AddDatesDialogExplanation)
            .SkipTo(TutorialStepIds.DateEntriesExplanation)
            .GoTo(TutorialStepIds.DateEntriesExplanation);

        builder.From(TutorialStepIds.DateEntriesExplanation)
            .GoTo(TutorialStepIds.ClickDateEntryStep);

        builder.From(TutorialStepIds.ClickDateEntryStep)
            .DisableBack()
            .SkipTo(TutorialStepIds.PointToSearchView)
            .GoTo(TutorialStepIds.EditDateEntryExplanation);

        builder.From(TutorialStepIds.EditDateEntryExplanation)
            .GoToIf(TutorialStepIds.SelectDatesExplanation, ctx => !ctx.HasCalendarEntries)
            .GoTo(TutorialStepIds.PointToSearchView);

        // Calendars -> Search
        builder.From(TutorialStepIds.PointToSearchView)
            .GoTo(TutorialStepIds.CalendarFilterExplanation);

        // Search flow
        builder.From(TutorialStepIds.CalendarFilterExplanation)
            .GoToIf(TutorialStepIds.CalendarTypesExplanation, ctx => ctx.HasCalendars)
            .GoTo(TutorialStepIds.SourcesFilterExplanation);

        builder.From(TutorialStepIds.CalendarTypesExplanation)
            .GoTo(TutorialStepIds.SourcesFilterExplanation);

        builder.From(TutorialStepIds.SourcesFilterExplanation)
            .DisableBack()
            .GoTo(TutorialStepIds.SelectSourceStep);

        builder.From(TutorialStepIds.SelectSourceStep)
            .GoTo(TutorialStepIds.SourceFilterOptionsExplanation);

        builder.From(TutorialStepIds.SourceFilterOptionsExplanation)
            .GoTo(TutorialStepIds.FiltersExplanation);

        builder.From(TutorialStepIds.FiltersExplanation)
            .DisableBack()
            .GoTo(TutorialStepIds.FiltersOptionsExplanation);

        builder.From(TutorialStepIds.FiltersOptionsExplanation)
            .GoTo(TutorialStepIds.SearchButtonExplanation);

        builder.From(TutorialStepIds.SearchButtonExplanation)
            .GoToIf(TutorialStepIds.ResultsExplanation, ctx => ctx.HasSearchResults)
            .GoTo(TutorialStepIds.NoResultsExplanation);

        builder.From(TutorialStepIds.ResultsExplanation)
            .GoTo(TutorialStepIds.TutorialComplete);

        builder.From(TutorialStepIds.NoResultsExplanation)
            .GoTo(TutorialStepIds.TutorialComplete);
    }
}
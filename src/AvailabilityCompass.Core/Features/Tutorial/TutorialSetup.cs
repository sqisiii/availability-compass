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
        // Introduction -> Sources
        builder.From(TutorialStepIds.Welcome)
            .GoToIf(TutorialStepIds.SourcesRefreshOptionalStep, ctx => ctx.CurrentDialog == DialogType.Sources && ctx.HasSourcesWithData)
            .GoToIf(TutorialStepIds.SourcesDialogRefreshButtons, ctx => ctx.CurrentDialog == DialogType.Sources)
            .GoTo(TutorialStepIds.PointToSourcesButton);

        // Sources flow
        builder.From(TutorialStepIds.PointToSourcesButton)
            .GoToIf(TutorialStepIds.SourcesRefreshOptionalStep, ctx => ctx.HasSourcesWithData)
            .GoTo(TutorialStepIds.SourcesDialogRefreshButtons);

        builder.From(TutorialStepIds.SourcesRefreshOptionalStep)
            .GoTo(TutorialStepIds.PointToCalendarsButton);

        builder.From(TutorialStepIds.SourcesDialogRefreshButtons)
            .GoToIf(TutorialStepIds.PointToCalendarsButton, ctx => ctx.HasRefreshedSource)
            .GoTo(TutorialStepIds.WaitForSourceRefresh);

        builder.From(TutorialStepIds.WaitForSourceRefresh)
            .GoTo(TutorialStepIds.PointToCalendarsButton);

        // Sources -> Calendars
        builder.From(TutorialStepIds.PointToCalendarsButton)
            .GoTo(TutorialStepIds.CalendarsOverview);

        // Calendars flow
        builder.From(TutorialStepIds.CalendarsOverview)
            .GoToIf(TutorialStepIds.ClickExistingCalendar, ctx => ctx.HasCalendars)
            .GoTo(TutorialStepIds.ClickAddCalendarButton);

        builder.From(TutorialStepIds.ClickExistingCalendar)
            .GoTo(TutorialStepIds.CalendarViewOverview);

        builder.From(TutorialStepIds.ClickAddCalendarButton)
            .GoTo(TutorialStepIds.AddCalendarFormExplanation);

        builder.From(TutorialStepIds.AddCalendarFormExplanation)
            .GoTo(TutorialStepIds.CalendarViewOverview);

        builder.From(TutorialStepIds.CalendarViewOverview)
            .GoTo(TutorialStepIds.EditCalendarStep);

        builder.From(TutorialStepIds.EditCalendarStep)
            .GoTo(TutorialStepIds.EditCalendarFormStep);

        builder.From(TutorialStepIds.EditCalendarFormStep)
            .GoTo(TutorialStepIds.DeleteCalendarStep);

        builder.From(TutorialStepIds.DeleteCalendarStep)
            .GoTo(TutorialStepIds.DeleteConfirmationStep);

        builder.From(TutorialStepIds.DeleteConfirmationStep)
            .GoToIf(TutorialStepIds.CalendarsOverview, ctx => !ctx.HasCalendars)
            .GoTo(TutorialStepIds.SelectDatesExplanation);

        builder.From(TutorialStepIds.SelectDatesExplanation)
            .GoTo(TutorialStepIds.AddDaysExplanation);

        builder.From(TutorialStepIds.AddDaysExplanation)
            .GoTo(TutorialStepIds.AddDatesDialogExplanation);

        builder.From(TutorialStepIds.AddDatesDialogExplanation)
            .GoTo(TutorialStepIds.DateEntriesExplanation);

        builder.From(TutorialStepIds.DateEntriesExplanation)
            .GoTo(TutorialStepIds.ClickDateEntryStep);

        builder.From(TutorialStepIds.ClickDateEntryStep)
            .GoTo(TutorialStepIds.EditDateEntryExplanation);

        builder.From(TutorialStepIds.EditDateEntryExplanation)
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
            .GoTo(TutorialStepIds.SelectSourceStep);

        builder.From(TutorialStepIds.SelectSourceStep)
            .GoTo(TutorialStepIds.SourceFilterOptionsExplanation);

        builder.From(TutorialStepIds.SourceFilterOptionsExplanation)
            .GoTo(TutorialStepIds.FiltersExplanation);

        builder.From(TutorialStepIds.FiltersExplanation)
            .GoTo(TutorialStepIds.FiltersOptionsExplanation);

        builder.From(TutorialStepIds.FiltersOptionsExplanation)
            .GoTo(TutorialStepIds.SearchButtonExplanation);

        builder.From(TutorialStepIds.SearchButtonExplanation)
            .GoTo(TutorialStepIds.ResultsExplanation);

        builder.From(TutorialStepIds.ResultsExplanation)
            .GoTo(TutorialStepIds.TutorialComplete);
    }
}
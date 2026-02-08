using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.AddDaysExplanation,
    Group = AppTutorialGroup.Calendars,
    Position = TooltipPosition.Right,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.All,
    Order = 6)]
[TutorialTarget("AddDaysButton")]
[TutorialTarget("CalendarWidget")]
[AutoAdvanceOn(AppTutorialTrigger.EditorOpened)]
public class AddDaysStep : ITutorialStepContent,
    IConditionalAutoAdvance<AvailabilityCompassContext, AppTutorialTrigger>,
    ITutorialStepSkippable<AvailabilityCompassContext>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.EditorOpened &&
           (previousContext == null || !previousContext.IsEditorOpen) &&
           currentContext.IsEditorOpen;

    public string Title => "Add Date Entries";

    public string Description => """
                                 You can still select more dates if you want.

                                 Click and drag on the calendar to select dates. Hold Ctrl for non-consecutive dates, or hold Shift to select a range.

                                 When you're ready, click the 'Add Dates' button to continue.
                                 """;

    public bool CanSkip(AvailabilityCompassContext context) => context.HasCalendarEntries;

    public string SkipTitle => "Add Date Entries";

    public string SkipDescription =>
        "You already have date entries in this calendar. Click 'Next' to continue, or add more dates if you'd like by pressing 'Add Dates' button.";
}
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
    ITutorialStepComplete<AvailabilityCompassContext>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.EditorOpened &&
           (previousContext == null || !previousContext.IsEditorOpen) &&
           currentContext.IsEditorOpen;

    public bool IsComplete(AvailabilityCompassContext context) => context.IsEditorOpen;

    public string Title => "Add Date Entries";

    public string Description => """
                                 You can still select more dates if you want.

                                 Click and drag on the calendar to select dates. Hold Ctrl for non-consecutive dates, or hold Shift to select a range.

                                 When you're ready, click the 'Add Dates' button to continue.
                                 """;
}
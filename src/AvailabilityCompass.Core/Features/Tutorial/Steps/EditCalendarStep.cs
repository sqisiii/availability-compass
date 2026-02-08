using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.EditCalendarStep,
    Group = AppTutorialGroup.Calendars,
    Position = TooltipPosition.Left,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.TargetOnly,
    Order = 6)]
[TutorialTarget("CalendarEditButton")]
[AutoAdvanceOn(AppTutorialTrigger.CalendarEditStarted)]
public class EditCalendarStep : ITutorialStepContent,
    IConditionalAutoAdvance<AvailabilityCompassContext, AppTutorialTrigger>,
    ITutorialStepSkippable<AvailabilityCompassContext>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.CalendarEditStarted && currentContext.IsEditCalendarExpanded;

    public string Title => "Edit Calendar";

    public string Description => """
                                 Click the Edit button to modify this calendar's settings.

                                 You can change the calendar name and toggle whether it only allows defined dates.
                                 """;

    public bool CanSkip(AvailabilityCompassContext context) => true;

    public string SkipTitle => "Edit Calendar";

    public string SkipDescription => "Click 'Edit' to see how calendar editing works, or press 'Next' to skip to adding dates.";
}
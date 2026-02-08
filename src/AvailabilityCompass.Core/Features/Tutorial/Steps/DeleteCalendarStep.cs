using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.DeleteCalendarStep,
    Group = AppTutorialGroup.Calendars,
    Position = TooltipPosition.Left,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.TargetOnly,
    Order = 8)]
[TutorialTarget("CalendarDeleteButton")]
[AutoAdvanceOn(AppTutorialTrigger.DeleteConfirmationOpened)]
public class DeleteCalendarStep : ITutorialStepContent,
    IConditionalAutoAdvance<AvailabilityCompassContext, AppTutorialTrigger>,
    ITutorialStepSkippable<AvailabilityCompassContext>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.DeleteConfirmationOpened && currentContext.IsDeleteConfirmationOpen;

    public string Title => "Delete Calendar";

    public string Description => """
                                 Click the Delete button to remove this calendar.

                                 A confirmation dialog will appear to prevent accidental deletion.
                                 """;

    public bool CanSkip(AvailabilityCompassContext context) => true;

    public string SkipTitle => "Delete Calendar";

    public string SkipDescription => "Click 'Delete' to see how calendar deletion works, or press 'Next' to skip to adding dates.";
}
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
    ITutorialStepComplete<AvailabilityCompassContext>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.DeleteConfirmationOpened && currentContext.IsDeleteConfirmationOpen;

    public bool IsComplete(AvailabilityCompassContext context) => context.IsDeleteConfirmationOpen;

    public string Title => "Delete Calendar";

    public string Description => """
                                 Click the Delete button to remove this calendar.

                                 A confirmation dialog will appear to prevent accidental deletion.
                                 """;
}
using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.DeleteConfirmationStep,
    Group = AppTutorialGroup.Calendars,
    Position = TooltipPosition.Right,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.All,
    Order = 9)]
[TutorialTarget("DeleteConfirmationDialog")]
[AutoAdvanceOn(AppTutorialTrigger.DeleteConfirmationClosed)]
public class DeleteConfirmationStep : ITutorialStepContent,
    IConditionalAutoAdvance<AvailabilityCompassContext, AppTutorialTrigger>,
    ITutorialStepComplete<AvailabilityCompassContext>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.DeleteConfirmationClosed && !currentContext.IsDeleteConfirmationOpen;

    public bool IsComplete(AvailabilityCompassContext context) => !context.IsDeleteConfirmationOpen;

    public string Title => "Confirm Deletion";

    public string Description => """
                                 This confirmation dialog prevents accidental deletions.

                                 • Cancel: Keep the calendar and return to the view
                                 • Delete: Permanently remove the calendar and all its date entries

                                 For this tutorial, click Cancel to keep your calendar.
                                 """;
}
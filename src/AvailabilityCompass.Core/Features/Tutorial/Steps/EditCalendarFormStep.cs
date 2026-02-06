using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.EditCalendarFormStep,
    Group = AppTutorialGroup.Calendars,
    Position = TooltipPosition.Right,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.All,
    Order = 7)]
[TutorialTarget("CalendarEditForm")]
[AutoAdvanceOn(AppTutorialTrigger.CalendarEditCompleted)]
public class EditCalendarFormStep : ITutorialStepContent,
    IConditionalAutoAdvance<AvailabilityCompassContext, AppTutorialTrigger>,
    ITutorialStepComplete<AvailabilityCompassContext>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.CalendarEditCompleted && !currentContext.IsEditCalendarExpanded;

    public bool IsComplete(AvailabilityCompassContext context) => !context.IsEditCalendarExpanded;

    public string Title => "Edit Calendar Form";

    public string Description => """
                                 This is the calendar edit form.

                                 • Name: Change the calendar's display name
                                 • Only allow defined dates: When checked, the calendar will only include the specific dates you add

                                 Click Save to apply your changes, or Cancel to discard them.
                                 """;
}
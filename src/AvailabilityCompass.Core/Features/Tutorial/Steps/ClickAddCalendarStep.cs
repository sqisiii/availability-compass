using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.ClickAddCalendarButton,
    Group = AppTutorialGroup.Calendars,
    Position = TooltipPosition.Left,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.TargetOnly,
    Order = 3)]
[TutorialTarget("AddCalendarButton")]
[AutoAdvanceOn(AppTutorialTrigger.CalendarFormExpanded)]
public class ClickAddCalendarStep : ITutorialStepContent,
    IConditionalAutoAdvance<AvailabilityCompassContext, AppTutorialTrigger>,
    ITutorialStepComplete<AvailabilityCompassContext>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.CalendarFormExpanded && currentContext.IsAddCalendarExpanded;

    public bool IsComplete(AvailabilityCompassContext context)
        => context.HasCalendars;

    public string Title => "Create Your First Calendar";

    public string Description => """
                                 Click the '+' button to create a new calendar.

                                 You'll be able to give it a name and choose whether it should mark:
                                 • Allowed days (dates you CAN travel)
                                 • Blocked days (dates you CANNOT travel)
                                 """;
}
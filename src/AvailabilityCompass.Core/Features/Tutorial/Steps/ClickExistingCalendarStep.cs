using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.ClickExistingCalendar,
    Group = AppTutorialGroup.Calendars,
    Position = TooltipPosition.Bottom,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.TargetOnly,
    Order = 2)]
[TutorialTarget("CalendarSelector")]
[AutoAdvanceOn(AppTutorialTrigger.CalendarSelected)]
public class ClickExistingCalendarStep : ITutorialStepContent,
    IConditionalAutoAdvance<AvailabilityCompassContext, AppTutorialTrigger>,
    ITutorialStepSkippable<AvailabilityCompassContext>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.CalendarSelected && currentContext.IsCalendarSelected;

    public string Title => "Select a Calendar";

    public string Description => """
                                 You already have calendars set up.

                                 Click on any calendar in this list to view and edit its dates. The calendar type (Allowed/Blocked) is shown below the name.

                                 The selected calendar's details will appear below.
                                 """;

    public bool CanSkip(AvailabilityCompassContext context) => context.IsCalendarSelected;

    public string SkipTitle => "Select a Calendar";

    public string SkipDescription => "A calendar is already selected. Click 'Next' to continue, or click another calendar to view its details.";
}
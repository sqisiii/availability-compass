using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.PointToCalendarsButton,
    Group = AppTutorialGroup.Calendars,
    Position = TooltipPosition.Bottom,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.TargetOnly,
    Order = 0)]
[TutorialTarget("CalendarsHeaderButton")]
[AutoAdvanceOn(AppTutorialTrigger.DialogChanged)]
public class PointToCalendarsStep : ITutorialStepContent, IConditionalAutoAdvance<AvailabilityCompassContext, AppTutorialTrigger>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.DialogChanged && currentContext.CurrentDialog == DialogType.Calendars;

    public string Title => "Set Up Your Availability";

    public string Description => """
                                 Excellent! Your trip data is now loaded.

                                 Next, let's set up your availability by creating calendars. Click the 'Calendars' button to define which dates you're available or unavailable for travel.
                                 """;
}
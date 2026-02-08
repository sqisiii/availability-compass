using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.PointToSearchView,
    Group = AppTutorialGroup.Calendars,
    Position = TooltipPosition.Bottom,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.All,
    Order = 13)]
[TutorialTarget("DialogCloseButton")]
[AutoAdvanceOn(AppTutorialTrigger.DialogChanged)]
public class PointToSearchStep : ITutorialStepContent, IConditionalAutoAdvance<AvailabilityCompassContext, AppTutorialTrigger>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.DialogChanged && currentContext.CurrentDialog == DialogType.None;

    public string Title => "Return to Search";

    public string Description => """
                                 Great job setting up your calendars!

                                 Now let's use them to find matching trips. Close this dialog to return to the main search view.
                                 """;
}
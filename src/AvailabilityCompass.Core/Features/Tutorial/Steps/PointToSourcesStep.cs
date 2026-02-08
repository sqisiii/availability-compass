using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.PointToSourcesButton,
    Group = AppTutorialGroup.Introduction,
    Position = TooltipPosition.Bottom,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.TargetOnly,
    Order = 1)]
[TutorialTarget("SourcesHeaderButton")]
[AutoAdvanceOn(AppTutorialTrigger.DialogChanged)]
public class PointToSourcesStep : ITutorialStepContent, IConditionalAutoAdvance<AvailabilityCompassContext, AppTutorialTrigger>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.DialogChanged && currentContext.CurrentDialog == DialogType.Sources;

    public string Title => "Open Sources Management";

    public string Description => """
                                 Click the 'Sources' button in the header to open the source management dialog.

                                 Here you can:
                                 • Refresh trip data from various providers
                                 • Enable or disable specific sources
                                 • See statistics about loaded data
                                 """;
}
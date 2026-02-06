using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.SourcesRefreshOptionalStep,
    Group = AppTutorialGroup.Sources,
    Position = TooltipPosition.Bottom,
    RequiresUserAction = false,
    ClickThroughMode = ClickThroughMode.TargetOnly,
    Order = 1)]
[TutorialTarget("RefreshAllButton")]
[TutorialTarget("SourceCardRefreshButton")]
[AutoAdvanceOn(AppTutorialTrigger.SourceRefreshed)]
public class SourcesRefreshOptionalStep : ITutorialStepContent,
    IConditionalAutoAdvance<AvailabilityCompassContext, AppTutorialTrigger>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.SourceRefreshed && currentContext.HasRefreshedSource;

    public string Title => "Refresh Your Trip Data";

    public string Description => """
                                 You already have trip data loaded from a previous session.

                                 You can either:
                                 • Click 'Refresh All' or a source's 'Refresh' button to get the latest data
                                 • Click 'Next' to continue with your existing data

                                 Refreshing ensures you have the most up-to-date trip information.
                                 """;
}

using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.SourcesDialogRefreshButtons,
    Group = AppTutorialGroup.Sources,
    Position = TooltipPosition.Bottom,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.TargetOnly,
    Order = 1)]
[TutorialTarget("RefreshAllButton")]
[TutorialTarget("SourceCardRefreshButton")]
[AutoAdvanceOn(AppTutorialTrigger.SourceRefreshed)]
public class SourcesRefreshStep : ITutorialStepContent, IConditionalAutoAdvance<AvailabilityCompassContext, AppTutorialTrigger>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.SourceRefreshed && currentContext.HasRefreshedSource;

    public string Title => "Refresh Your Trip Data";

    public string Description => """
                                 Each source card represents a different trip provider.

                                 To load the latest trip data, you can either:
                                 • Click 'Refresh All' button at the top to update all sources at once
                                 • Click the 'Refresh' button on individual source cards to update them one by one

                                 The progress bar shows the download status. Once refreshed, you'll see how many trip records were loaded and when they were last updated.
                                 """;
}
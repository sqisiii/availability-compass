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
public class SourcesRefreshStep : ITutorialStepContent,
    IConditionalAutoAdvance<AvailabilityCompassContext, AppTutorialTrigger>,
    ITutorialStepSkippable<AvailabilityCompassContext>
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

    public bool CanSkip(AvailabilityCompassContext context) => context.HasSourcesWithData;

    public string SkipTitle => "Refresh Your Trip Data";

    public string SkipDescription => """
                                     You already have trip data loaded from a previous session.

                                     You can either:
                                     • Click 'Refresh All' or a source's 'Refresh' button to get the latest data
                                     • Click 'Next' to continue with your existing data

                                     Refreshing ensures you have the most up-to-date trip information.
                                     """;
}
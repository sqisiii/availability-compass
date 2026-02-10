using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.WaitForSourceRefresh,
    Group = AppTutorialGroup.Sources,
    Position = TooltipPosition.Bottom,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.TargetOnly,
    Order = 2)]
[TutorialTarget("RefreshAllButton")]
[AutoAdvanceOn(AppTutorialTrigger.SourceRefreshed)]
public class WaitForRefreshStep : ITutorialStepContent,
    IConditionalAutoAdvance<AvailabilityCompassContext, AppTutorialTrigger>,
    ITutorialStepSkippable<AvailabilityCompassContext>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.SourceRefreshed && currentContext.HasRefreshedSource;

    public string Title => "Refreshing Sources...";

    public string Description => """
                                 Your trip data is being downloaded. Please wait for the refresh to complete.

                                 You can see the progress on each source card.
                                 """;

    public bool CanSkip(AvailabilityCompassContext context)
        => context.HasRefreshedSource || context.HasSourcesWithData;

    public string SkipTitle => "Refreshing Sources...";

    public string SkipDescription =>
        "You already have trip data loaded. Click 'Next' to continue to the calendars setup, or wait for the current refresh to finish.";
}
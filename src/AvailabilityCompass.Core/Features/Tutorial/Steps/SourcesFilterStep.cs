using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.SourcesFilterExplanation,
    Group = AppTutorialGroup.Search,
    Position = TooltipPosition.Bottom,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.TargetOnly,
    Order = 2)]
[TutorialTarget("SourcesFilterSection")]
[AutoAdvanceOn(AppTutorialTrigger.FilterSelected)]
public class SourcesFilterStep : ITutorialStepContent, IConditionalAutoAdvance<AvailabilityCompassContext, AppTutorialTrigger>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.FilterSelected && currentContext.HasSourceFilterSelected;

    public string Title => "Filter by Source";

    public string Description => """
                                 Click 'Sources' to expand this section.

                                 Toggle source cards to include or exclude specific trip providers from your search results.

                                 Disabled sources (those you turned off in source management) appear grayed out.
                                 """;
}
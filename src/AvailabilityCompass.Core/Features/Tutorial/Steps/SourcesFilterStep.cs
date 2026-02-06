using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.SourcesFilterExplanation,
    Group = AppTutorialGroup.Search,
    Position = TooltipPosition.Bottom,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.TargetOnly,
    Order = 2)]
[TutorialTarget("SourcesFilterSection")]
[AutoAdvanceOn(AppTutorialTrigger.SourcesFilterExpanded)]
public class SourcesFilterStep : ITutorialStepContent, IConditionalAutoAdvance<AvailabilityCompassContext, AppTutorialTrigger>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.SourcesFilterExpanded && currentContext.IsSourcesFilterExpanded;

    public string Title => "Filter by Source";

    public string Description => "Click 'Sources' to expand this section and view available trip providers.";
}
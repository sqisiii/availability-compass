using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.FiltersExplanation,
    Group = AppTutorialGroup.Search,
    Position = TooltipPosition.Bottom,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.TargetOnly,
    Order = 5)]
[TutorialTarget("FiltersSection")]
[AutoAdvanceOn(AppTutorialTrigger.FiltersExpanded)]
public class FiltersStep : ITutorialStepContent, IConditionalAutoAdvance<AvailabilityCompassContext, AppTutorialTrigger>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.FiltersExpanded && currentContext.IsFiltersExpanded;

    public string Title => "Global Filters";

    public string Description => "Click 'Filters' to expand this section and access global search options.";
}
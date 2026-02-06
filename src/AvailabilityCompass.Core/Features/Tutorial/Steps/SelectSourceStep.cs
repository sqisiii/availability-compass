using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.SelectSourceStep,
    Group = AppTutorialGroup.Search,
    Position = TooltipPosition.Bottom,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.TargetOnly,
    Order = 3)]
[TutorialTarget("SourceCards")]
[AutoAdvanceOn(AppTutorialTrigger.FilterSelected)]
public class SelectSourceStep : ITutorialStepContent, IConditionalAutoAdvance<AvailabilityCompassContext, AppTutorialTrigger>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.FilterSelected && currentContext.HasSourceFilterSelected;

    public string Title => "Select a Source";

    public string Description => """
                                 Click on a source card to include it in your search.

                                 You can select multiple sources to search across different trip providers.
                                 """;
}

using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.FiltersOptionsExplanation,
    Group = AppTutorialGroup.Search,
    Position = TooltipPosition.Bottom,
    ClickThroughMode = ClickThroughMode.TargetOnly,
    Order = 6)]
[TutorialTarget("GlobalFilterFields")]
public class FiltersOptionsStep : ITutorialStepContent
{
    public string Title => "Filter Options";

    public string Description => """
                                 Available filter options:

                                 • Search phrase: Find trips containing specific text in their title or description
                                 • Start Date / End Date: Limit results to trips within a specific date range

                                 These filters apply across all selected sources.
                                 """;
}

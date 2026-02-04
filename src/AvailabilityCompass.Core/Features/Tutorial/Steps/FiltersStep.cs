using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.FiltersExplanation,
    Group = AppTutorialGroup.Search,
    Position = TooltipPosition.Bottom,
    Order = 4)]
[TutorialTarget("FiltersSection")]
public class FiltersStep : ITutorialStepContent
{
    public string Title => "Global Filters";

    public string Description => """
                                 Click 'Filters' to access global search options:

                                 • Search phrase: Find trips containing specific text in their title or description
                                 • Start Date / End Date: Limit results to trips within a specific date range

                                 These filters apply across all selected sources.
                                 """;
}
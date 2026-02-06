using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.ResultsExplanation,
    Group = AppTutorialGroup.Search,
    Position = TooltipPosition.Bottom,
    Order = 6)]
[TutorialTarget("ResultsSection")]
public class ResultsStep : ITutorialStepContent
{
    public string Title => "Search Results";

    public string Description => """
                                 After clicking 'Search', matching trips appear as cards below.

                                 Each card shows:
                                 • Source name and language
                                 • Destination and travel dates
                                 • Trip title and details
                                 • Additional information specific to each source

                                 Click on any result card to open the trip's page on the source website in your browser. You can also sort results using the dropdown at the top right.
                                 """;
}
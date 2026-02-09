using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.NoResultsExplanation,
    Group = AppTutorialGroup.Search,
    Position = TooltipPosition.Bottom,
    Order = 6)]
[TutorialTarget("NoResultsSection")]
public class NoResultsStep : ITutorialStepContent
{
    public string Title => "No Results Found";

    public string Description => """
                                 Your search didn't return any matching trips. This can happen when your calendar dates don't overlap with available trips, or your filters are too restrictive.

                                 You can try adjusting your filters or calendar settings and search again after completing the tutorial.

                                 Click 'Next' to continue.
                                 """;
}
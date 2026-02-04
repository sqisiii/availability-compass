using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.TutorialComplete,
    Group = AppTutorialGroup.Complete,
    Position = TooltipPosition.Center,
    Order = 0)]
public class TutorialCompleteStep : ITutorialStepContent
{
    public string Title => "Tutorial Complete!";

    public string Description => """
                                 Congratulations! You've learned how to:

                                 ✓ Refresh trip data from sources
                                 ✓ Create and manage availability calendars
                                 ✓ Add one-time and recurring date entries
                                 ✓ Search for trips matching your schedule
                                 ✓ Filter results by source and criteria

                                 You can restart this tutorial anytime from the header menu.

                                 Happy trip hunting!
                                 """;
}
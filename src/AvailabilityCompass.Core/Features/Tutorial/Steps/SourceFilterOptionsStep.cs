using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.SourceFilterOptionsExplanation,
    Group = AppTutorialGroup.Search,
    Position = TooltipPosition.Bottom,
    ClickThroughMode = ClickThroughMode.TargetOnly,
    Order = 4)]
[TutorialTarget("SourceFilterOptions")]
public class SourceFilterOptionsStep : ITutorialStepContent
{
    public string Title => "Source-Specific Filters";

    public string Description => """
                                 After selecting sources, additional filter options may appear below each source card.

                                 These source-specific filters let you narrow down results based on criteria unique to each trip provider (e.g., destination type, trip category).
                                 """;
}
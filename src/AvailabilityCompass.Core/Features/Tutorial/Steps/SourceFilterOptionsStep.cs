using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.SourceFilterOptionsExplanation,
    Group = AppTutorialGroup.Search,
    Position = TooltipPosition.Bottom,
    Order = 3)]
[TutorialTarget("SourceFilterOptions")]
public class SourceFilterOptionsStep : ITutorialStepContent
{
    public string Title => "Source-Specific Filters";

    public string Description => """
                                 When you select a source, additional filter options may appear below.

                                 Each source has different criteria you can filter by, such as:
                                 • Destination preferences
                                 • Trip type (cruise, tour, etc.)
                                 • Price ranges
                                 • Other source-specific options

                                 These filters are optional and help narrow down results.
                                 """;
}
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
                                 Toggle source cards to include or exclude specific trip providers from your search results.

                                 Disabled sources (those you turned off in source management) appear grayed out.

                                 When you select a source, additional filter options may appear below with source-specific criteria.
                                 """;
}
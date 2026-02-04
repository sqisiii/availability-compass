using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.SearchButtonExplanation,
    Group = AppTutorialGroup.Search,
    Position = TooltipPosition.Bottom,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.TargetOnly,
    Order = 5)]
[TutorialTarget("SearchButton")]
[AutoAdvanceOn(AppTutorialTrigger.SearchPerformed)]
public class SearchButtonStep : ITutorialStepContent, IConditionalAutoAdvance<AvailabilityCompassContext, AppTutorialTrigger>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.SearchPerformed &&
           (previousContext == null || !previousContext.HasSearchResults) &&
           currentContext.HasSearchResults;

    public string Title => "Run Your Search";

    public string Description => """
                                 Click the 'Search' button to find trips matching your criteria.

                                 The search will query all selected sources and filter results based on your calendar and filter settings.
                                 """;
}
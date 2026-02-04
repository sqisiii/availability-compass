using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.WaitForSourceRefresh,
    Group = AppTutorialGroup.Sources,
    Position = TooltipPosition.Bottom,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.TargetOnly,
    Order = 2)]
[TutorialTarget("SourcesHeaderButton")]
[AutoAdvanceOn(AppTutorialTrigger.SourceRefreshed)]
public class WaitForRefreshStep : ITutorialStepContent,
    IConditionalAutoAdvance<AvailabilityCompassContext, AppTutorialTrigger>,
    ITutorialStepComplete<AvailabilityCompassContext>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.SourceRefreshed && currentContext.HasRefreshedSource;

    public bool IsComplete(AvailabilityCompassContext context)
        => context.HasRefreshedSource || context.HasSourcesWithData;

    public string Title => "Refresh Sources First";

    public string Description => """
                                 Before setting up calendars, please refresh at least one source to load trip data.

                                 Click the 'Sources' button and use the Refresh buttons to download trip information.
                                 """;
}
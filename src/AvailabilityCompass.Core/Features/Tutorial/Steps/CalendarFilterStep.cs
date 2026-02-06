using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.CalendarFilterExplanation,
    Group = AppTutorialGroup.Search,
    Position = TooltipPosition.Bottom,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.TargetOnly,
    Order = 1)]
[TutorialTarget("CalendarFilterSection")]
[AutoAdvanceOn(AppTutorialTrigger.CalendarFilterExpanded)]
public class CalendarFilterStep : ITutorialStepContent, IConditionalAutoAdvance<AvailabilityCompassContext, AppTutorialTrigger>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.CalendarFilterExpanded && currentContext.IsCalendarFilterExpanded;

    public string Title => "Filter by Calendar";

    public string Description => """
                                 Click 'Calendars' to expand this section and view available calendar filters.
                                 """;
}
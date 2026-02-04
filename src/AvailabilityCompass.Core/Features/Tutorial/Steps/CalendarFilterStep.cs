using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.CalendarFilterExplanation,
    Group = AppTutorialGroup.Search,
    Position = TooltipPosition.Bottom,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.TargetOnly,
    Order = 1)]
[TutorialTarget("CalendarFilterSection")]
[AutoAdvanceOn(AppTutorialTrigger.FilterSelected)]
public class CalendarFilterStep : ITutorialStepContent, IConditionalAutoAdvance<AvailabilityCompassContext, AppTutorialTrigger>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.FilterSelected && currentContext.HasCalendarFilterSelected;

    public string Title => "Filter by Calendar";

    public string Description => """
                                 Click 'Calendars' to expand this section and select which calendars to apply when searching.

                                 You can select multiple calendars:
                                 • Green checkmark calendars = Allowed days (results must fall within these dates)
                                 • Orange checkmark calendars = Blocked days (results will exclude these dates)

                                 If no calendars are selected, all dates are considered available.
                                 """;
}
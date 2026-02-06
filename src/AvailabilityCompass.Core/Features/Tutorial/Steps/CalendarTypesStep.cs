using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.CalendarTypesExplanation,
    Group = AppTutorialGroup.Search,
    Position = TooltipPosition.Bottom,
    ClickThroughMode = ClickThroughMode.TargetOnly,
    Order = 2)]
[TutorialTarget("CalendarCards")]
public class CalendarTypesStep : ITutorialStepContent
{
    public string Title => "Calendar Filter Types";

    public string Description => """
                                 Select which calendars to apply when searching:

                                 • Green checkmark calendars = Allowed days (results must fall within these dates)
                                 • Orange checkmark calendars = Blocked days (results will exclude these dates)

                                 If no calendars are selected, all dates are considered available.
                                 """;
}
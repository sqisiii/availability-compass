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
    public string Title => "Calendar Filter Modes";

    public string Description => """
                                 Select calendars and choose how they behave:

                                 • Green ✓ icon calendars = Available days (results must fall within these dates)
                                 • Teal ⊘ icon calendars = Blocked days (results will exclude these dates)

                                 Use the F/M toggle on each card to switch modes:
                                 • F Filter mode: calendar affects search results (default)
                                 • M Mark-only mode: calendar does not filter, but shows conflict days on each result card

                                 If no calendars are selected for filtering, all dates are considered available.
                                 """;
}
using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.CalendarsOverview,
    Group = AppTutorialGroup.Calendars,
    Position = TooltipPosition.Center,
    Order = 1)]
public class CalendarsOverviewStep : ITutorialStepContent
{
    public string Title => "Understanding Calendars";

    public string Description => """
                                 Calendars help you define your availability for searching trips. There are two types:

                                 • Allowed Days Calendar: Only dates you add to this calendar will be considered when searching. Use this to mark specific vacation days or available periods.

                                 • Blocked Days Calendar: Dates you add will be excluded from search results. Use this to mark busy days, work commitments, or dates you cannot travel.

                                 You can create multiple calendars of each type and combine them when searching.
                                 """;
}
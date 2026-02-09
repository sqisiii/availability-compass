using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.CalendarsOverview,
    Group = AppTutorialGroup.Calendars,
    Position = TooltipPosition.Center,
    Order = 1)]
public class CalendarsOverviewStep : ITutorialStepContent
{
    public string Title => "Calendars Overview";

    public string Description => """
                                 This section lets you manage calendars that define your availability when searching for trips. There are two types:

                                 • Available Days Calendar: Only dates you add will be considered when searching. Use this to mark vacation days or available periods.

                                 • Blocked Days Calendar: Dates you add will be excluded from search results. Use this to mark busy days or dates you cannot travel.

                                 You can create multiple calendars of each type and combine them when searching.

                                 Click 'Next' to start setting up your calendars.
                                 """;
}
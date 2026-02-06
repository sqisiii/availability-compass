using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.CalendarViewOverview,
    Group = AppTutorialGroup.Calendars,
    Position = TooltipPosition.Right,
    ClickThroughMode = ClickThroughMode.None,
    Order = 5)]
[TutorialTarget("CalendarSection")]
public class CalendarViewOverviewStep : ITutorialStepContent
{
    public string Title => "Managing Calendar Dates";

    public string Description => """
                                 Now you can add dates to your calendar:

                                 • Edit button: Change the calendar name or type
                                 • Delete button: Remove this calendar entirely
                                 • Calendar widget: Click and drag to select dates, then use 'Add Dates' to save them

                                 The legend shows color coding:
                                 • Dark red = One-time dates
                                 • Salmon = Recurring dates

                                 Click on any colored date to edit that entry.
                                 """;
}
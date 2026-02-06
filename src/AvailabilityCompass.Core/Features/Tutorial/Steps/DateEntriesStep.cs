using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.DateEntriesExplanation,
    Group = AppTutorialGroup.Calendars,
    Position = TooltipPosition.Left,
    Order = 7)]
[TutorialTarget("DateEntriesPanel")]
public class DateEntriesStep : ITutorialStepContent
{
    public string Title => "View and Edit Entries";

    public string Description => """
                                 The Date Entries panel on the right shows all dates added to this calendar.

                                 Each entry displays:
                                 • The description you provided
                                 • The date or date range
                                 • Recurring pattern info (if applicable)

                                 Entries can be edited by clicking on them or by clicking colored dates in the calendar widget.
                                 """;
}
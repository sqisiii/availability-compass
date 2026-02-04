using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.AddDaysExplanation,
    Group = AppTutorialGroup.Calendars,
    Position = TooltipPosition.Left,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.All,
    Order = 6)]
[TutorialTarget("AddDaysButton")]
[AutoAdvanceOn(AppTutorialTrigger.DateEntryAdded)]
public class AddDaysStep : ITutorialStepContent,
    IConditionalAutoAdvance<AvailabilityCompassContext, AppTutorialTrigger>,
    ITutorialStepComplete<AvailabilityCompassContext>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.DateEntryAdded &&
           (previousContext == null || !previousContext.HasCalendarEntries) &&
           currentContext.HasCalendarEntries;

    public bool IsComplete(AvailabilityCompassContext context)
        => context.HasCalendarEntries;

    public string Title => "Add Date Entries";

    public string Description => """
                                 After selecting dates on the calendar, click 'Add Dates' to create an entry.

                                 In the popup:
                                 • Description: Add a note explaining this date range (e.g., 'Family reunion', 'Conference')

                                 • 'Make this recurring' checkbox: Enable this for dates that repeat regularly. You can then set:
                                   - Frequency: How many days between occurrences (e.g., 7 for weekly, 14 for bi-weekly)
                                   - Repetitions: How many times it should repeat

                                 This is useful for regular commitments like weekly meetings or monthly events.
                                 """;
}
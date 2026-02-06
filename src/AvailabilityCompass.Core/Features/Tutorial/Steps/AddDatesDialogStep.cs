using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.AddDatesDialogExplanation,
    Group = AppTutorialGroup.Calendars,
    Position = TooltipPosition.Right,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.All,
    Order = 7)]
[TutorialTarget("DateEntryEditorPanel")]
[AutoAdvanceOn(AppTutorialTrigger.DateEntryAdded)]
public class AddDatesDialogStep : ITutorialStepContent,
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

    public bool IsComplete(AvailabilityCompassContext context) => context.HasCalendarEntries;

    public string Title => "Configure Date Entry";

    public string Description => """
                                 Fill in the date entry details:

                                 • Description: Add a note explaining this date range (e.g., 'Family reunion', 'Conference')

                                 • 'Make this recurring' checkbox: Enable this for dates that repeat regularly. You can then set:
                                   - Repeat after (days): How many days until the next occurrence (e.g., 7 for weekly, 14 for bi-weekly)
                                   - Repetitions: Total number of times the entry will repeat

                                 Click 'Save' when you're done.
                                 """;
}
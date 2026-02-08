using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.AddCalendarFormExplanation,
    Group = AppTutorialGroup.Calendars,
    Position = TooltipPosition.Bottom,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.All,
    Order = 4)]
[TutorialTarget("AddCalendarForm")]
[AutoAdvanceOn(AppTutorialTrigger.CalendarAdded)]
public class AddCalendarFormStep : ITutorialStepContent,
    IConditionalAutoAdvance<AvailabilityCompassContext, AppTutorialTrigger>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.CalendarAdded &&
           (previousContext == null || !previousContext.HasCalendars) &&
           currentContext.HasCalendars;

    public string Title => "Configure Your Calendar";

    public string Description => """
                                 Fill in the calendar details:

                                 • Calendar Name: Enter a descriptive name (e.g., 'Summer Vacation', 'Work Trips', 'Blocked Weekends')

                                 • 'Only allow defined dates' checkbox:
                                   ✓ Checked = Allowed Days Calendar - Only the dates you add will be available for searching
                                   ☐ Unchecked = Blocked Days Calendar - The dates you add will be excluded from searches

                                 Click 'Create' when ready, or 'Cancel' to go back.
                                 """;
}
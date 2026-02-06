using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.SelectDatesExplanation,
    Group = AppTutorialGroup.Calendars,
    Position = TooltipPosition.Right,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.All,
    Order = 6)]
[TutorialTarget("CalendarWidget")]
[AutoAdvanceOn(AppTutorialTrigger.DatesSelected)]
public class SelectDatesStep : ITutorialStepContent,
    IConditionalAutoAdvance<AvailabilityCompassContext, AppTutorialTrigger>,
    ITutorialStepComplete<AvailabilityCompassContext>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.DatesSelected && currentContext.HasSelectedDates;

    public bool IsComplete(AvailabilityCompassContext context) => context.HasSelectedDates;

    public string Title => "Select Dates";

    public string Description => """
                                 Click and drag on the calendar to select the dates you want to add.

                                 You can select multiple dates at once by clicking and dragging, hold Ctrl to select non-consecutive dates, or hold Shift to select a range of dates in a row.

                                 Selected dates will be highlighted.
                                 """;
}
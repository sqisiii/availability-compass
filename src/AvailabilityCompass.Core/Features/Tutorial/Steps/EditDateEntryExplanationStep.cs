using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.EditDateEntryExplanation,
    Group = AppTutorialGroup.Calendars,
    Position = TooltipPosition.Right,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.TargetOnly,
    Order = 9)]
[TutorialTarget("DateEntryEditorPanel")]
[AutoAdvanceOn(AppTutorialTrigger.EditorClosed)]
public class EditDateEntryExplanationStep : ITutorialStepContent,
    IConditionalAutoAdvance<AvailabilityCompassContext, AppTutorialTrigger>,
    ITutorialStepComplete<AvailabilityCompassContext>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.EditorClosed && !currentContext.IsEditorOpen;

    public bool IsComplete(AvailabilityCompassContext context) => !context.IsEditorOpen;

    public string Title => "Edit Date Entry";

    public string Description => """
                                 From here you can make changes to your date entry:

                                 - Change the description to update the note for this entry
                                 - Modify the date range if needed
                                 - Enable or disable recurring settings
                                 - Adjust frequency and repetitions for recurring entries
                                 - Delete the entry using the delete button

                                 Click 'Save' to apply changes or 'Cancel' to discard them.
                                 """;
}
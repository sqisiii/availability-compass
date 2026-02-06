using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.ClickDateEntryStep,
    Group = AppTutorialGroup.Calendars,
    Position = TooltipPosition.Left,
    RequiresUserAction = true,
    ClickThroughMode = ClickThroughMode.TargetOnly,
    Order = 8)]
[TutorialTarget("DateEntryItems")]
[AutoAdvanceOn(AppTutorialTrigger.EditorOpened)]
public class ClickDateEntryStep : ITutorialStepContent,
    IConditionalAutoAdvance<AvailabilityCompassContext, AppTutorialTrigger>,
    ITutorialStepComplete<AvailabilityCompassContext>
{
    public bool ShouldAutoAdvance(
        AppTutorialTrigger trigger,
        AvailabilityCompassContext? previousContext,
        AvailabilityCompassContext currentContext)
        => trigger == AppTutorialTrigger.EditorOpened && currentContext.IsEditorOpen;

    public bool IsComplete(AvailabilityCompassContext context) => context.IsEditorOpen;

    public string Title => "Click an Entry";

    public string Description => """
                                 Click on any date entry in the list to open the editor.

                                 This will allow you to view and modify the entry's details.
                                 """;
}
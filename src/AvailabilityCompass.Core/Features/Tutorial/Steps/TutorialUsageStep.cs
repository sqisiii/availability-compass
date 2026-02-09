using Guidely.Core.Abstractions;

namespace AvailabilityCompass.Core.Features.Tutorial.Steps;

[TutorialStep(TutorialStepIds.TutorialUsage,
    Group = AppTutorialGroup.Introduction,
    Position = TooltipPosition.Center,
    Order = 1)]
public class TutorialUsageStep : ITutorialStepContent
{
    public string Title => "How This Tutorial Works";

    public string Description => """
                                 A few things to know before we begin:

                                 • Some steps require you to perform an action — look for the colored banner at the bottom of this dialog.

                                 • Other steps are informational — just click 'Next' to continue.

                                 • Use 'Back' to revisit previous steps.

                                 • You can drag this dialog if it covers something you need to see.
                                 """;
}
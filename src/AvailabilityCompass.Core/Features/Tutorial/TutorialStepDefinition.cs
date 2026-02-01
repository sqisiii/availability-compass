namespace AvailabilityCompass.Core.Features.Tutorial;

/// <summary>
/// Defines a single step in the tutorial with its content and behavior.
/// </summary>
/// <param name="Id">Unique identifier for this step.</param>
/// <param name="Title">Short title displayed in the tooltip header.</param>
/// <param name="Description">Detailed description with markdown-like formatting support.</param>
/// <param name="PrimaryTarget">Main UI element to highlight.</param>
/// <param name="SecondaryTarget">Optional second element to highlight (for steps pointing to multiple elements).</param>
/// <param name="Position">Where to position the tooltip relative to the target.</param>
/// <param name="RequiresUserAction">If true, step advances on user interaction; if false, only on Next click.</param>
public record TutorialStepDefinition(
    TutorialStepId Id,
    string Title,
    string Description,
    TutorialTargetElement PrimaryTarget,
    TutorialTargetElement? SecondaryTarget,
    TooltipPosition Position,
    bool RequiresUserAction = false
);
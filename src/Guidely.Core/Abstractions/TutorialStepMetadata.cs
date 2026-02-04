namespace Guidely.Core.Abstractions;

/// <summary>
/// Resolved metadata for a tutorial step, combining attribute data with step instance.
/// </summary>
/// <typeparam name="TTrigger">The trigger enum type.</typeparam>
/// <typeparam name="TGroup">The group enum type.</typeparam>
/// <param name="Id">Unique identifier for this step.</param>
/// <param name="Title">The title of the step.</param>
/// <param name="Description">The description/body of the step.</param>
/// <param name="Group">The group this step belongs to.</param>
/// <param name="Order">Order of this step within its group.</param>
/// <param name="Targets">Target element names for this step.</param>
/// <param name="Position">Position of the tooltip.</param>
/// <param name="RequiresUserAction">Whether user action is required.</param>
/// <param name="ClickThroughMode">Click-through mode for the overlay.</param>
/// <param name="AutoAdvanceTriggers">Triggers that can cause auto-advance.</param>
/// <param name="StepInstance">The instantiated step object.</param>
public record TutorialStepMetadata<TTrigger, TGroup>(
    string Id,
    string Title,
    string Description,
    TGroup Group,
    int Order,
    IReadOnlyList<string> Targets,
    TooltipPosition Position,
    bool RequiresUserAction,
    ClickThroughMode ClickThroughMode,
    IReadOnlyList<TTrigger> AutoAdvanceTriggers,
    // ReSharper disable once NotAccessedPositionalProperty.Global
    object StepInstance
) where TTrigger : Enum where TGroup : Enum;
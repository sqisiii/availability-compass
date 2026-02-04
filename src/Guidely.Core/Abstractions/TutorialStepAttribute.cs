namespace Guidely.Core.Abstractions;

/// <summary>
/// Marks a class as a tutorial step with metadata.
/// </summary>
/// <param name="id">Unique identifier for this step.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class TutorialStepAttribute(string id) : Attribute
{
    /// <summary>
    /// Unique identifier for this step.
    /// </summary>
    public string Id { get; } = id;

    /// <summary>
    /// The group this step belongs to.
    /// Pass your group enum value directly (e.g., MyGroup.Introduction).
    /// </summary>
    public object? Group { get; set; }

    /// <summary>
    /// Position of the tooltip relative to the target element.
    /// </summary>
    public TooltipPosition Position { get; set; } = TooltipPosition.Bottom;

    /// <summary>
    /// Whether this step requires user action to advance (vs. allowing the Next button).
    /// </summary>
    public bool RequiresUserAction { get; set; }

    /// <summary>
    /// Click-through mode for the overlay.
    /// </summary>
    public ClickThroughMode ClickThroughMode { get; set; } = ClickThroughMode.None;

    /// <summary>
    /// Order of this step within its group. Lower values come first.
    /// </summary>
    public int Order { get; set; }
}
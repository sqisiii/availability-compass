namespace Guidely.Core.Abstractions;

/// <summary>
/// Determines how click-through behavior works for a tutorial step.
/// </summary>
public enum ClickThroughMode
{
    /// <summary>
    /// No click-through allowed - overlay blocks all interaction.
    /// </summary>
    None,

    /// <summary>
    /// Only the target element(s) can be clicked through.
    /// </summary>
    TargetOnly,

    /// <summary>
    /// All elements can be clicked through the overlay.
    /// </summary>
    All
}
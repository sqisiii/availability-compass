namespace AvailabilityCompass.Core.Features.Tutorial;

/// <summary>
/// Defines how the tutorial overlay handles click-through behavior.
/// </summary>
public enum ClickThroughMode
{
    /// <summary>
    /// Backdrop blocks all clicks. User must use tooltip buttons (Next/Back/Skip).
    /// </summary>
    NoneClickable,

    /// <summary>
    /// Only highlighted target elements are clickable. Clicks elsewhere are blocked.
    /// </summary>
    OnlyHighlightedClickable,

    /// <summary>
    /// Entire backdrop allows click-through. All underlying UI is interactive.
    /// </summary>
    AllClickable
}

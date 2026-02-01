namespace AvailabilityCompass.Core.Features.Tutorial;

/// <summary>
/// Position of the tutorial tooltip relative to the target element.
/// </summary>
public enum TooltipPosition
{
    Top,
    Bottom,
    Left,
    Right,
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
    Center // For steps without a target element
}
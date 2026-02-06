using System.Windows;
using System.Windows.Media;
using Guidely.Core.Abstractions;

namespace Guidely.WPF.Controls;

/// <summary>
/// Calculates tooltip position relative to target elements.
/// </summary>
public class TooltipPositioner
{
    private static double Margin => 24;

    /// <summary>
    /// Calculates the tooltip position based on target element and desired placement.
    /// </summary>
    public Point CalculatePosition(
        FrameworkElement? targetElement,
        Size tooltipSize,
        Size overlaySize,
        TooltipPosition position,
        Window? window,
        Visual overlay)
    {
        if (targetElement == null || position == TooltipPosition.Center || window == null)
        {
            return CalculateCenteredPosition(tooltipSize, overlaySize);
        }

        try
        {
            var targetBounds = GetElementBoundsRelativeToOverlay(targetElement, window, overlay);
            var calculatedPosition = CalculatePositionRelativeToTarget(targetBounds, tooltipSize, position);
            return ClampToVisibleArea(calculatedPosition, tooltipSize, overlaySize);
        }
        catch
        {
            return CalculateCenteredPosition(tooltipSize, overlaySize);
        }
    }

    /// <summary>
    /// Clamps a position to keep the tooltip within the visible area.
    /// </summary>
    public Point ClampToVisibleArea(Point position, Size tooltipSize, Size overlaySize)
    {
        var left = Math.Max(Margin, Math.Min(position.X, overlaySize.Width - tooltipSize.Width - Margin));
        var top = Math.Max(Margin, Math.Min(position.Y, overlaySize.Height - tooltipSize.Height - Margin));
        return new Point(left, top);
    }

    /// <summary>
    /// Gets the bounds of an element relative to the overlay using the window as common coordinate space.
    /// </summary>
    public static Rect GetElementBoundsRelativeToOverlay(FrameworkElement element, Window window, Visual overlay)
    {
        var elementToWindow = element.TransformToAncestor(window);
        var elementBoundsInWindow = elementToWindow.TransformBounds(new Rect(element.RenderSize));
        var overlayInWindow = overlay.TransformToAncestor(window).Transform(new Point(0, 0));

        return elementBoundsInWindow with
        {
            X = elementBoundsInWindow.X - overlayInWindow.X,
            Y = elementBoundsInWindow.Y - overlayInWindow.Y
        };
    }

    private Point CalculateCenteredPosition(Size tooltipSize, Size overlaySize)
    {
        var left = (overlaySize.Width - tooltipSize.Width) / 2;
        var top = (overlaySize.Height - tooltipSize.Height) / 2;
        return ClampToVisibleArea(new Point(left, top), tooltipSize, overlaySize);
    }

    private Point CalculatePositionRelativeToTarget(Rect targetBounds, Size tooltipSize, TooltipPosition position)
    {
        double left = 0, top = 0;

        switch (position)
        {
            case TooltipPosition.Top:
                left = targetBounds.X + targetBounds.Width / 2 - tooltipSize.Width / 2;
                top = targetBounds.Y - tooltipSize.Height - Margin;
                break;

            case TooltipPosition.Bottom:
                left = targetBounds.X + targetBounds.Width / 2 - tooltipSize.Width / 2;
                top = targetBounds.Y + targetBounds.Height + Margin;
                break;

            case TooltipPosition.Left:
                left = targetBounds.X - tooltipSize.Width - Margin;
                top = targetBounds.Y + targetBounds.Height / 2 - tooltipSize.Height / 2;
                break;

            case TooltipPosition.Right:
                left = targetBounds.X + targetBounds.Width + Margin;
                top = targetBounds.Y + targetBounds.Height / 2 - tooltipSize.Height / 2;
                break;
        }

        return new Point(left, top);
    }
}
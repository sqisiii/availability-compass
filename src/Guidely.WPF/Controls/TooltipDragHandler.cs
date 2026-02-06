using System.Windows;
using System.Windows.Input;

namespace Guidely.WPF.Controls;

/// <summary>
/// Handles drag-to-reposition behavior for a tooltip element.
/// </summary>
public class TooltipDragHandler
{
    private readonly Func<Size> _getOverlaySize;
    private readonly TooltipPositioner _positioner;
    private readonly FrameworkElement _tooltip;
    private Point _dragStartMousePosition;

    private bool _isDragging;
    private Point _tooltipStartPosition;

    public TooltipDragHandler(
        FrameworkElement tooltip,
        Func<Size> getOverlaySize,
        TooltipPositioner positioner)
    {
        _tooltip = tooltip;
        _getOverlaySize = getOverlaySize;
        _positioner = positioner;
    }

    public void Attach()
    {
        _tooltip.MouseLeftButtonDown += OnMouseDown;
        _tooltip.MouseLeftButtonUp += OnMouseUp;
        _tooltip.MouseMove += OnMouseMove;
    }

    public void Detach()
    {
        _tooltip.MouseLeftButtonDown -= OnMouseDown;
        _tooltip.MouseLeftButtonUp -= OnMouseUp;
        _tooltip.MouseMove -= OnMouseMove;
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        _isDragging = true;
        _dragStartMousePosition = e.GetPosition((IInputElement)_tooltip.Parent);
        _tooltipStartPosition = new Point(_tooltip.Margin.Left, _tooltip.Margin.Top);
        _tooltip.CaptureMouse();
        e.Handled = true;
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (!_isDragging)
        {
            return;
        }

        _isDragging = false;
        _tooltip.ReleaseMouseCapture();
        e.Handled = true;
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        if (!_isDragging)
        {
            return;
        }

        var currentPosition = e.GetPosition((IInputElement)_tooltip.Parent);
        var delta = currentPosition - _dragStartMousePosition;

        var newLeft = _tooltipStartPosition.X + delta.X;
        var newTop = _tooltipStartPosition.Y + delta.Y;

        var tooltipSize = new Size(
            _tooltip.ActualWidth > 0 ? _tooltip.ActualWidth : _tooltip.Width,
            _tooltip.ActualHeight > 0 ? _tooltip.ActualHeight : _tooltip.Height);

        var clamped = _positioner.ClampToVisibleArea(
            new Point(newLeft, newTop),
            tooltipSize,
            _getOverlaySize());

        _tooltip.Margin = new Thickness(clamped.X, clamped.Y, 0, 0);
    }
}
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Guidely.Core.Abstractions;

namespace Guidely.WPF.Controls;

/// <summary>
/// A Border control that supports selective hit-testing for tutorial overlays.
/// Allows click-through based on the configured mode and target elements.
/// Renders the backdrop with transparent cutouts for highlighted elements.
/// </summary>
public class SelectiveHitTestBorder : Border
{
    /// <summary>
    /// Padding around cutout areas (larger than a highlight border for visual breathing room).
    /// </summary>
    private const double CutoutPadding = 16;

    /// <summary>
    /// Identifies the ClickThroughMode dependency property.
    /// </summary>
    public static readonly DependencyProperty ClickThroughModeProperty = DependencyProperty.Register(
        nameof(ClickThroughMode),
        typeof(ClickThroughMode),
        typeof(SelectiveHitTestBorder),
        new PropertyMetadata(ClickThroughMode.None));

    /// <summary>
    /// Gets or sets the click-through mode for this border.
    /// </summary>
    public ClickThroughMode ClickThroughMode
    {
        get => (ClickThroughMode)GetValue(ClickThroughModeProperty);
        set => SetValue(ClickThroughModeProperty, value);
    }

    /// <summary>
    /// The list of target elements that should be clickable when in TargetOnly mode.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public List<FrameworkElement> ClickableTargets { get; } = [];

    /// <summary>
    /// Updates the list of clickable targets.
    /// </summary>
    public void SetClickableTargets(params FrameworkElement?[] targets)
    {
        ClickableTargets.Clear();

        foreach (var target in targets)
        {
            if (target != null)
            {
                ClickableTargets.Add(target);
            }
        }
    }

    /// <summary>
    /// Overrides the default hit-test behavior to support selective click-through.
    /// </summary>
    protected override HitTestResult? HitTestCore(PointHitTestParameters hitTestParameters)
    {
        switch (ClickThroughMode)
        {
            case ClickThroughMode.All:
                // All clicks pass through - return null to indicate no hit
                return null;

            case ClickThroughMode.TargetOnly:
                // Check if the click is within any of the target element bounds
                return IsPointInClickableTarget(hitTestParameters.HitPoint)
                    ?
                    // Click is in a target area - allow pass-through
                    null
                    :
                    // Click is outside targets - block it
                    base.HitTestCore(hitTestParameters);

            case ClickThroughMode.None:
            default:
                // All clicks are blocked - return a normal hit-test result
                return base.HitTestCore(hitTestParameters);
        }
    }

    /// <summary>
    /// Checks if the given point (in this border's coordinate space) is within any clickable target.
    /// Uses Window coordinate space to handle targets in different visual trees (e.g., dialogs).
    /// </summary>
    private bool IsPointInClickableTarget(Point hitPoint)
    {
        if (ClickableTargets.Count == 0)
        {
            return false;
        }

        // Get the window to use as a common coordinate space
        var window = Window.GetWindow(this);
        if (window == null)
        {
            return false;
        }

        // Transform the hit point to window coordinates
        Point hitPointInWindow;
        try
        {
            hitPointInWindow = TransformToAncestor(window).Transform(hitPoint);
        }
        catch (InvalidOperationException)
        {
            return false;
        }

        foreach (var target in ClickableTargets)
        {
            if (!target.IsLoaded || !target.IsVisible)
            {
                continue;
            }

            try
            {
                // Transform target bounds to window coordinates
                var targetTransform = target.TransformToAncestor(window);
                var targetBounds = targetTransform.TransformBounds(new Rect(target.RenderSize));

                if (targetBounds.Contains(hitPointInWindow))
                {
                    return true;
                }
            }
            catch (InvalidOperationException)
            {
                // Target is not connected to the window - skip it
            }
        }

        return false;
    }

    /// <summary>
    /// Updates the OpacityMask to create transparent cutouts for target elements.
    /// Call this after updating clickable targets.
    /// </summary>
    public void InvalidateCutouts()
    {
        if (ClickableTargets.Count == 0 || ActualWidth <= 0 || ActualHeight <= 0)
        {
            OpacityMask = null;
            return;
        }

        // Create geometry: full rectangle minus cutouts
        var fullRect = new RectangleGeometry(new Rect(0, 0, ActualWidth, ActualHeight));
        Geometry maskGeometry = fullRect;

        foreach (var target in ClickableTargets)
        {
            var bounds = GetTargetBoundsRelativeToThis(target);
            if (bounds.HasValue)
            {
                var cutoutRect = new RectangleGeometry(bounds.Value);
                maskGeometry = new CombinedGeometry(
                    GeometryCombineMode.Exclude,
                    maskGeometry,
                    cutoutRect);
            }
        }

        // White = opaque, transparent areas (excluded) = holes
        var drawing = new GeometryDrawing(Brushes.White, null, maskGeometry);
        OpacityMask = new DrawingBrush(drawing)
        {
            Stretch = Stretch.None,
            AlignmentX = AlignmentX.Left,
            AlignmentY = AlignmentY.Top
        };
    }

    /// <summary>
    /// Gets the bounds of a target element relative to this border's coordinate space.
    /// </summary>
    private Rect? GetTargetBoundsRelativeToThis(FrameworkElement target)
    {
        if (!target.IsLoaded || !target.IsVisible)
        {
            return null;
        }

        try
        {
            var targetPosition = target.TransformToVisual(this).Transform(new Point(0, 0));
            var bounds = new Rect(
                targetPosition.X - CutoutPadding,
                targetPosition.Y - CutoutPadding,
                target.RenderSize.Width + CutoutPadding * 2,
                target.RenderSize.Height + CutoutPadding * 2);
            return bounds;
        }
        catch (InvalidOperationException)
        {
            // Target is not in the same visual tree
            return null;
        }
    }
}
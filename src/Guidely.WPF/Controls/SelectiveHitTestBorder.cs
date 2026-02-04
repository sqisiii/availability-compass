using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Guidely.Core.Abstractions;

namespace Guidely.WPF.Controls;

/// <summary>
/// A Border control that supports selective hit-testing for tutorial overlays.
/// Allows click-through based on the configured mode and target elements.
/// </summary>
public class SelectiveHitTestBorder : Border
{
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
}
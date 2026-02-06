using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

// ReSharper disable MemberCanBePrivate.Global

namespace Guidely.WPF.Controls;

/// <summary>
/// Manages tutorial highlight rectangles on a canvas.
/// </summary>
public class HighlightManager
{
    private readonly Canvas _canvas;
    private readonly List<(string Name, Rectangle Highlight, Storyboard Animation)> _highlights = [];

    public HighlightManager(Canvas canvas)
    {
        _canvas = canvas;
    }

    public HighlightStyle Style { get; set; } = HighlightStyle.Default;

    /// <summary>
    /// Updates highlights for the given targets.
    /// </summary>
    /// <returns>List of clickable target elements for backdrop cutouts.</returns>
    public IReadOnlyList<FrameworkElement> UpdateHighlights(
        IReadOnlyList<string> targets,
        Window? window,
        Visual overlay,
        TooltipPositioner positioner)
    {
        Clear();

        if (window == null)
        {
            return [];
        }

        var clickableTargets = new List<FrameworkElement>();

        foreach (var targetName in targets)
        {
            var elements = TutorialElementRegistry.GetElements(targetName);

            foreach (var element in elements)
            {
                clickableTargets.Add(element);

                try
                {
                    var bounds = TooltipPositioner.GetElementBoundsRelativeToOverlay(element, window, overlay);
                    var position = new Point(bounds.X, bounds.Y);
                    var size = new Size(bounds.Width, bounds.Height);

                    var (highlight, animation) = CreateHighlightRectangle(position, size);
                    _canvas.Children.Add(highlight);
                    _highlights.Add((targetName, highlight, animation));
                }
                catch
                {
                    // Transform failed, skip this element
                }
            }
        }

        return clickableTargets;
    }

    /// <summary>
    /// Clears all highlights and stops animations.
    /// </summary>
    public void Clear()
    {
        foreach (var (_, _, animation) in _highlights)
        {
            animation.Stop();
        }

        _highlights.Clear();
        _canvas.Children.Clear();
    }

    private (Rectangle Highlight, Storyboard Animation) CreateHighlightRectangle(Point position, Size size)
    {
        var highlight = new Rectangle
        {
            Width = size.Width + HighlightStyle.Padding * 2,
            Height = size.Height + HighlightStyle.Padding * 2,
            Stroke = new SolidColorBrush(Style.Color),
            StrokeThickness = HighlightStyle.BorderThickness,
            RadiusX = HighlightStyle.CornerRadius,
            RadiusY = HighlightStyle.CornerRadius,
            Fill = Brushes.Transparent,
            IsHitTestVisible = false
        };

        Canvas.SetLeft(highlight, position.X - HighlightStyle.Padding);
        Canvas.SetTop(highlight, position.Y - HighlightStyle.Padding);

        var animation = new DoubleAnimation
        {
            From = HighlightStyle.OpacityFrom,
            To = HighlightStyle.OpacityTo,
            Duration = Style.AnimationDuration,
            AutoReverse = true,
            RepeatBehavior = RepeatBehavior.Forever,
            EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
        };

        var storyboard = new Storyboard();
        storyboard.Children.Add(animation);
        Storyboard.SetTarget(animation, highlight);
        Storyboard.SetTargetProperty(animation, new PropertyPath(UIElement.OpacityProperty));
        storyboard.Begin();

        return (highlight, storyboard);
    }
}
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;

// ReSharper disable MemberCanBePrivate.Global

namespace AvailabilityCompass.WpfClient.Shared.Controls;

/// <summary>
/// Adorner that draws a pulsing highlight border around a tutorial target element.
/// </summary>
public class TutorialHighlightAdorner : Adorner
{
    public static readonly DependencyProperty AnimatedOpacityProperty =
        DependencyProperty.Register(
            nameof(AnimatedOpacity),
            typeof(double),
            typeof(TutorialHighlightAdorner),
            new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender,
                (d, e) => ((TutorialHighlightAdorner)d)._animatedOpacity = (double)e.NewValue));

    private readonly Storyboard _pulseAnimation;
    private double _animatedOpacity = 1.0;

    public TutorialHighlightAdorner(UIElement adornedElement) : base(adornedElement)
    {
        IsHitTestVisible = false;

        // Create pulse animation
        _pulseAnimation = CreatePulseAnimation();
        _pulseAnimation.Begin(this, true);
    }

    /// <summary>
    /// The highlight color (primary blue by default).
    /// </summary>
    public Color HighlightColor { get; set; } = Color.FromRgb(59, 130, 246);

    /// <summary>
    /// The border thickness.
    /// </summary>
    public double BorderThickness { get; set; } = 3;

    /// <summary>
    /// The corner radius of the highlight border.
    /// </summary>
    public double CornerRadius { get; set; } = 8;

    /// <summary>
    /// Padding around the adorned element.
    /// </summary>
    public double Padding { get; set; } = 4;

    /// <summary>
    /// Animated opacity for the pulse effect.
    /// </summary>
    public double AnimatedOpacity
    {
        get => _animatedOpacity;
        set
        {
            _animatedOpacity = value;
            InvalidateVisual();
        }
    }

    private Storyboard CreatePulseAnimation()
    {
        var animation = new DoubleAnimation
        {
            From = 1.0,
            To = 0.4,
            Duration = TimeSpan.FromMilliseconds(800),
            AutoReverse = true,
            RepeatBehavior = RepeatBehavior.Forever,
            EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
        };

        var storyboard = new Storyboard();
        storyboard.Children.Add(animation);

        Storyboard.SetTarget(animation, this);
        Storyboard.SetTargetProperty(animation, new PropertyPath(nameof(AnimatedOpacity)));

        return storyboard;
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        var rect = new Rect(AdornedElement.RenderSize);
        rect.Inflate(Padding, Padding);

        // Update pen opacity for pulse effect
        var brush = new SolidColorBrush(HighlightColor) { Opacity = _animatedOpacity };
        var pen = new Pen(brush, BorderThickness);

        // Draw a rounded rectangle border
        var geometry = new RectangleGeometry(rect, CornerRadius, CornerRadius);
        drawingContext.DrawGeometry(null, pen, geometry);

        // Draw a subtle glow effect
        var glowBrush = new SolidColorBrush(HighlightColor) { Opacity = _animatedOpacity * 0.2 };
        var glowRect = rect;
        glowRect.Inflate(2, 2);
        var glowGeometry = new RectangleGeometry(glowRect, CornerRadius + 2, CornerRadius + 2);
        var glowPen = new Pen(glowBrush, 6);
        drawingContext.DrawGeometry(null, glowPen, glowGeometry);
    }

    /// <summary>
    /// Stops the animation and removes the adorner.
    /// </summary>
    public void StopAnimation()
    {
        _pulseAnimation.Stop(this);
    }
}